document.addEventListener("DOMContentLoaded", () => {
    const btnLoad = document.getElementById("btn-load-cards");
    const inputId = document.getElementById("search-resa");
    const container = document.getElementById("container-cartes");
    const template = document.getElementById("template-carte");

    //variable du pour fonctionnement du tableau
    let cartesGenerees = [];
    let indexCarteActuelle = 0;

    btnLoad.addEventListener("click", () => {
        const idResa = inputId.value;
        if (idResa) chargerCartes(idResa);
    });

    async function chargerCartes(id) {
        try {
            container.innerHTML = "<p>Chargement...</p>";
            cartesGenerees = []; // Reset

            //infos Réservations
            const resp = await fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}`);
            if (!resp.ok) throw new Error("Réservation introuvable");
            const resa = await resp.json();

            //généré les cartes Passagers
            for (let i = 1; i <= resa.nbPassagers; i++) {
                const pResp = await fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}/passager/${i}`);
                const p = await pResp.json();
                ajouterCarteMemoire(resa, p, "Passager");
            }

            //générer les cartes Véhicules
            for (let i = 1; i <= resa.nbVehicules; i++) {
                const vResp = await fetch(`https://tomorow.eu.pythonanywhere.com/CAN/api/reservation/${id}/vehicule/${i}`);
                const v = await vResp.json();
                ajouterCarteMemoire(resa, v, "Véhicule");
            }

            // afficher la premiere carte (première carte par défaut)
            afficherCarte(0);

        } catch (error) { //Si il y a une erreure avec le fetch (réservation inéxistante) (ligne 23)
            container.innerHTML = `<p style="color:red">Erreur: ${error.message}</p>`;
        }
    }

    function ajouterCarteMemoire(resa, item, type) {
        // On stocke les donnés pour les afficher une par une
        cartesGenerees.push({ resa, item, type });
    }

    function afficherCarte(index) {
        if (cartesGenerees.length === 0) {
            container.innerHTML = "<p>Aucune carte à afficher.</p>";
            return;
        }

        // Gestion des limites
        if (index < 0) index = cartesGenerees.length - 1;
        if (index >= cartesGenerees.length) index = 0;
        indexCarteActuelle = index;

        const data = cartesGenerees[index];
        const clone = template.content.cloneNode(true);

        // Remplissage commun à toute les cartes
        clone.querySelector(".card-depart").textContent = data.resa.portDepart;
        clone.querySelector(".card-arrivee").textContent = data.resa.portArrivee;
        clone.querySelector(".card-date").textContent = data.resa.date;
        clone.querySelector(".card-heure").textContent = data.resa.heure;
        clone.querySelector(".card-bateau").textContent = data.resa.bateau;
        clone.querySelector(".card-ref").textContent = data.resa.id;
        clone.querySelector(".card-client").textContent = data.resa.nom;

        // Remplissage spécifique Passager ou Véhicule
        const typeLabel = clone.querySelector(".card-type-label");
        const nomObjet = clone.querySelector(".card-nom-objet");
        const nomLabel = clone.querySelector(".card-nom-label");
        const prenomSeul = clone.querySelector(".card-prenom-seul");
        const categorie = clone.querySelector(".card-categorie");
        const prix = clone.querySelector(".card-prix");
        const nombre = clone.querySelector(".card-quantity");

        if (data.type === "Passager") {
            typeLabel.textContent = "Passager";
            if(nomLabel) nomLabel.textContent = "Nom";
            
            //affichage nom
            nomObjet.textContent = data.item.nom; 
            
            //affichage prenom 
            if(prenomSeul) {
                prenomSeul.textContent = data.item.prenom;
                prenomSeul.parentElement.style.display = "flex";
                nombre.parentElement.style.display = "none";
            }

            categorie.textContent = data.item.libelleCategorie || data.item.code;
            prix.textContent = data.item.price.toFixed(2);
        } else {
            typeLabel.textContent = "Véhicule";
            if(nomLabel) nomLabel.textContent = "Type";
            
            nomObjet.textContent = data.item.libelle || data.item.code;
            
            // Pour un véhicule, pas de prénom, on cache la ligne
            if(prenomSeul) {
                prenomSeul.textContent = "";
                prenomSeul.parentElement.style.display = "none";
                categorie.parentElement.style.display = "none";
            }

            nombre.textContent = data.item.quantite;
            prix.textContent = (data.item.prix * data.item.quantite).toFixed(2);
        }

        const qrData = `Ici on pourrais mettre un lien ou : \n${data.resa.id}e Reservation \nCarte de type : ${data.type}`;
        
        // Appel de l'API pour générer l'image (taille 150x150, marge 0)
        const qrUrl = `https://api.qrserver.com/v1/create-qr-code/?size=150x150&margin=10&data=${encodeURIComponent(qrData)}`;
        
        // Insertion de l'URL dans la balise <img>
        const imgQr = clone.querySelector(".card-qrcode-img");
        if(imgQr) imgQr.src = qrUrl;

        // Ajout des boutons de navigation dynamiquement
        const divNav = document.createElement("div");
        divNav.className = "navigation-cartes";
        divNav.style.marginTop = "20px";
        divNav.innerHTML = `
            <button onclick="changerCarte(-1)">&#8592; Précédent</button>
            <span>Carte ${index + 1} / ${cartesGenerees.length}</span>
            <button onclick="changerCarte(1)">Suivant &#8594;</button>
        `;

        // Affichage
        container.innerHTML = ""; // On efface la précédente
        container.appendChild(clone);
        container.appendChild(divNav);
    }

    // Rendre la fonction accessible globalement pour les boutons onclick
    window.changerCarte = function(next) {
        afficherCarte(indexCarteActuelle + next);
    };
});