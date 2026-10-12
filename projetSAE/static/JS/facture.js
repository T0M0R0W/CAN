document.addEventListener("DOMContentLoaded", () => {
    const btnLoad = document.getElementById("btn-load-facture");
    const inputId = document.getElementById("facture-id-input");
    const container = document.getElementById("facture-content");

    btnLoad.addEventListener("click", () => {
        const idResa = inputId.value;
        if (!idResa) {
            alert("Veuillez entrer un numéro de réservation.");
            return;
        }
        chargerFacture(idResa);
    });

    async function chargerFacture(id) {
        try {
            const responseResa = await fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}`);
            if (!responseResa.ok) throw new Error("Réservation introuvable");
            const resa = await responseResa.json();

            document.getElementById("res-id").textContent = resa.id;
            document.getElementById("res-nom").textContent = resa.nom;
            document.getElementById("res-depart").textContent = resa.portDepart;
            document.getElementById("res-arrivee").textContent = resa.portArrivee;
            document.getElementById("res-date").textContent = resa.date;
            document.getElementById("res-heure").textContent = resa.heure;
            document.getElementById("res-bateau").textContent = resa.bateau;
            
            document.getElementById("date-edition").textContent = new Date().toLocaleDateString("fr-FR");

            // --- TRAITEMENT DES PASSAGERS (REGROUPEMENT) ---
            const tbodyPassagers = document.getElementById("tbody-passagers");
            tbodyPassagers.innerHTML = "";
            let totalPassagers = 0;

            const promessesPassagers = [];
            for (let i = 1; i <= resa.nbPassagers; i++) {
                promessesPassagers.push(fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}/passager/${i}`).then(r => r.json()));
            }
            const passagers = await Promise.all(promessesPassagers);

            // On utilise un objet pour regrouper par catégorie
            const groupes = {};

            passagers.forEach(p => {
                const nomCat = p.libelleCategorie || p.code; 
                if (!groupes[nomCat]) {
                    groupes[nomCat] = {
                        label: nomCat,
                        nombre: 0,
                        prixUnitaire: p.price,
                        prixTotal: 0
                    };
                }
                groupes[nomCat].nombre++;
                groupes[nomCat].prixTotal += p.price;
            });

            // On affiche chaque groupe
            for (const key in groupes) {
                const g = groupes[key];
                const tr = document.createElement("tr");
                
                // Si le prix est 0, on écrit "gratuit"
                const prixUnitAffiche = g.prixUnitaire > 0 ? g.prixUnitaire.toFixed(2) + " €" : "gratuit";
                const prixTotalAffiche = g.prixTotal > 0 ? g.prixTotal.toFixed(2) + " €" : "0 €";

                tr.innerHTML = `
                    <td>${g.label}</td>
                    <td>${g.nombre}</td>
                    <td>${prixUnitAffiche}</td>
                    <td>${prixTotalAffiche}</td>
                `;
                tbodyPassagers.appendChild(tr);
                totalPassagers += g.prixTotal;
            }
            document.getElementById("total-passagers").textContent = totalPassagers.toFixed(2) + " €";


            // --- TRAITEMENT DES VÉHICULES ---
            const tbodyVehicules = document.getElementById("tbody-vehicules");
            tbodyVehicules.innerHTML = "";
            let totalVehicules = 0;

            const promessesVehicules = [];
            for (let i = 1; i <= resa.nbVehicules; i++) {
                promessesVehicules.push(fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}/vehicule/${i}`).then(r => r.json()));
            }
            const vehicules = await Promise.all(promessesVehicules);

            vehicules.forEach(v => {
                const tr = document.createElement("tr");
                const prixLigne = v.quantite * v.prix;
                tr.innerHTML = `
                    <td>${v.libelle || v.code}</td>
                    <td>${v.quantite}</td>
                    <td>${v.prix.toFixed(2)} €</td>
                    <td>${prixLigne.toFixed(2)} €</td> `;
                tbodyVehicules.appendChild(tr);
                totalVehicules += prixLigne;
            });
            document.getElementById("total-vehicules").textContent = totalVehicules.toFixed(2) + " €";

            const grandTotal = totalPassagers + totalVehicules;
            document.getElementById("grand-total").textContent = grandTotal.toFixed(2);

            container.style.display = "block";

        } catch (error) {
            console.error(error);
            alert("Erreur : " + error.message);
        }
    }
});