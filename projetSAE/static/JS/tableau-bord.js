document.addEventListener("DOMContentLoaded", () => {
    const btn = document.getElementById("btn-voir");
    const container = document.getElementById("resultats-remplissage");

    btn.addEventListener("click", async () => {
        const idLiaison = document.getElementById("select-liaison").value;
        const date = document.getElementById("input-date").value;

        if (!date) {
            alert("Veuillez choisir une date.");
            return;
        }

        container.innerHTML = "<p>Chargement...</p>";

        try {
            // Appel API
            const url = `https://tomorow.eu.pythonanywhere.com/CAN/api/liaison/${idLiaison}/remplissage/${date}`;
            const response = await fetch(url);
            
            if (!response.ok) throw new Error("Erreur de récupération");
            
            const traversees = await response.json();

            container.innerHTML = ""; // Vider

            if (traversees.length === 0) {
                container.innerHTML = "<p>Aucune traversée trouvée pour cette date.</p>";
                return;
            }

            // Générer l'affichage pour chaque traversée
            traversees.forEach(t => {
                // Calcul des pourcentages
                const pctPassagers = (t.nbReservationPassagers / t.capacitePassagers) * 100;
                const pctVehicules = (t.nbReservationVoitures / t.capaciteVoitures) * 100;

                const html = `
                <div class="carte-traversee">
                    <div class="heure-traversee">${t.heure}</div>
                    
                    <div class="conteneur-jauges">
                        <div class="ligne-jauge">
                            <span class="label-jauge">Passagers</span>
                            <div class="barre-fond">
                                <div class="barre-remplissage ${getColorClass(pctPassagers)}" 
                                     style="width: ${Math.min(pctPassagers, 100)}%;"></div>
                            </div>
                            <span class="pourcentage-texte">${pctPassagers.toFixed(1)}%</span>
                        </div>

                        <div class="ligne-jauge">
                            <span class="label-jauge">Véhicules</span>
                            <div class="barre-fond">
                                <div class="barre-remplissage ${getColorClass(pctVehicules)}" 
                                     style="width: ${Math.min(pctVehicules, 100)}%;"></div>
                            </div>
                            <span class="pourcentage-texte">${pctVehicules.toFixed(1)}%</span>
                        </div>
                    </div>
                </div>`;
                
                container.innerHTML += html;
            });

        } catch (error) {
            console.error(error);
            container.innerHTML = "<p style='color:red'>Erreur lors du chargement des données.</p>";
        }
    });

    function getColorClass(pct) {
        if (pct > 100) return "bg-fluo"; // Jaune fluo au delà
        if (pct >= 75) return "bg-rouge"; // Rouge de 75 à 100
        if (pct >= 50) return "bg-orange"; // Orange entre 50 et 75
        return "bg-vert"; // Vert jusqu'à 50
    }
});