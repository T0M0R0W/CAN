document.addEventListener("DOMContentLoaded", () => {
    const btnLoad = document.getElementById("btn-load-stats");
    const container = document.getElementById("stats-container");
    const template = document.getElementById("template-stat");

    btnLoad.addEventListener("click", chargerStatistiques);

    async function chargerStatistiques() {
        try {
            container.innerHTML = "<p>Chargement des données...</p>";
            
            // 1. Récupérer la liste des liaisons
            const responseLiaisons = await fetch("https://can.iutrs.unistra.fr/api/liaison/all");
            if (!responseLiaisons.ok) throw new Error("Impossible de récupérer les liaisons");
            const liaisons = await responseLiaisons.json();

            container.innerHTML = ""; // Vider le message de chargement

            let grandTotalCA = 0;

            // Pour chaque liaison, récupérer le CA
            // On utilise une boucle for...of pour gérer l'async/await proprement
            for (const liaison of liaisons) {
                const respCA = await fetch(`https://can.iutrs.unistra.fr/api/liaison/${liaison.id}/chiffreAffaire`);
                const stats = await respCA.json();

                // Créer la carte HTML
                const clone = template.content.cloneNode(true);

                clone.querySelector(".stat-titre").textContent = stats.nom;

                // Passagers
                clone.querySelector(".stat-nb-pass").textContent = stats.passagers.nombre;
                clone.querySelector(".stat-ca-pass").textContent = stats.passagers.chiffreAffaire.toFixed(2);

                // Véhicules
                clone.querySelector(".stat-nb-veh").textContent = stats.vehicules.quantite;
                clone.querySelector(".stat-ca-veh").textContent = stats.vehicules.chiffreAffaire.toFixed(2);

                // Total Liaison
                const totalLiaison = stats.passagers.chiffreAffaire + stats.vehicules.chiffreAffaire;
                clone.querySelector(".stat-total").textContent = totalLiaison.toFixed(2);
                
                grandTotalCA += totalLiaison;

                container.appendChild(clone);
            }

            // Ajouter un récapitulatif global en bas
            const recapDiv = document.createElement("div");
            recapDiv.style.width = '100%'
            recapDiv.innerHTML = `
                <div style="width:100%; text-align:center; margin: 20px 0; font-size: 1.5em; font-weight:bold; color: #2c3e50;">
                    Chiffre d'affaires Global : ${grandTotalCA.toFixed(2)} €
                </div>
            `;
            container.appendChild(recapDiv);

        } catch (error) {
            console.error(error);
            container.innerHTML = `<p style="color:red">Erreur : ${error.message}</p>`;
        }
    }
});