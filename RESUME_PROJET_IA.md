# Résumé du Projet : Bataille de Drones IA

## 1. Aperçu du Projet

Ce projet Unity est une simulation de bataille en temps réel entre deux armées (Rouge et Verte) entièrement contrôlées par une intelligence artificielle. Le champ de bataille est composé de différentes unités avec des rôles spécialisés :

*   **Drones (au sol) :** Unités d'attaque de base.
*   **Drones Volants :** Unités de support (guérisseurs) ou d'attaque.
*   **Tourelles :** Unités de défense statiques avec une puissance de feu importante (dégâts de zone).

L'IA de chaque unité est gérée par des arbres de comportement via le plugin **Behavior Designer**, permettant de créer des logiques de décision complexes et modulaires.

## 2. Objectif Principal

L'objectif de nos interventions était de transformer l'IA d'un état réactif et basique à un système capable d'exécuter des **stratégies complexes et coordonnées**. L'enjeu était d'itérer sur la logique de prise de décision des unités pour augmenter drastiquement leur efficacité au combat et leurs chances de victoire.

## 3. Résumé des Stratégies et Modifications Implémentées

Nous avons travaillé sur deux axes stratégiques principaux : l'amélioration du support avec les drones guérisseurs et l'optimisation de l'offensive avec les drones d'attaque.

### Stratégie de l'Armée Verte (Drones Guérisseurs)

L'objectif était de transformer le drone volant en un véritable atout de survie pour son armée.

1.  **Fiabilisation du Soin :** La première étape a été de corriger un bug qui empêchait le drone de trouver le composant `Health` de ses alliés, rendant le soin fonctionnel.
2.  **Optimisation de la Mobilité :** La vitesse du drone a été augmentée et sa trajectoire de vol a été rendue plus directe pour minimiser le temps de réaction entre la détection d'un allié blessé et l'application du soin.
3.  **Ciblage de Soin Intelligent (`FindWeakestAlly.cs`) :** Pour maximiser l'impact de chaque soin, une logique de score a été implémentée. Le drone choisit désormais sa cible en priorité en fonction de deux critères pondérés :
    *   **Santé la plus basse :** L'allié le plus proche de la destruction est prioritaire.
    *   **Proximité :** Un allié plus proche est soigné plus vite.

### Stratégie de l'Armée Rouge (Drones d'Attaque)

L'offensive a été entièrement revue pour passer d'une attaque désorganisée à une offensive stratégique et méthodique.

1.  **Priorisation des Menaces (`FindBestTurretTarget.cs`) :** Conscient que les tourelles ennemies représentaient la plus grande menace avec leurs dégâts de zone, une stratégie en deux phases a été mise en place :
    *   **Phase 1 :** Tous les drones concentrent leurs tirs exclusivement sur les tourelles jusqu'à ce qu'elles soient toutes détruites.
    *   **Phase 2 :** Une fois les tourelles neutralisées, les drones passent à l'attaque des drones ennemis restants.
2.  **Sélection de Cible Avancée (`FindBest...Target.cs`) :** Que ce soit pour une tourelle ou un drone, la cible est choisie selon un score pondéré (santé basse + proximité) pour achever rapidement les ennemis les plus vulnérables.
3.  **Coordination et Anti-Regroupement :** Pour éviter que toute l'armée ne se concentre sur une seule et même cible (gaspillage de dégâts), les drones choisissent désormais leur cible au hasard parmi les **3 cibles les plus pertinentes**, assurant une meilleure répartition des tirs.
4.  **Amélioration de la Poursuite et de l'Engagement :**
    *   **Ténacité (`HasTarget.cs`) :** Un drone "verrouille" sa cible et ne la changera pas tant qu'elle n'est pas détruite, évitant la dispersion.
    *   **Poursuite Fluide (`SeekAndFaceTarget.cs`) :** Contre les cibles mobiles, les tâches de déplacement et de rotation ont été fusionnées en une seule action continue. Le drone poursuit et vise sa cible simultanément, rendant ses attaques beaucoup plus précises et efficaces.

## 4. Conclusion

À travers ces itérations successives, l'IA est passée d'un comportement basique à une exécution de stratégies avancées. Les unités coopèrent, hiérarchisent les menaces, se répartissent les cibles et poursuivent leurs ennemis de manière tenace et efficace. Le résultat est une armée bien plus redoutable, capable de s'adapter à la situation du champ de bataille pour maximiser ses chances de succès.
