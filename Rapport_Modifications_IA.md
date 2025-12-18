# Rapport des Modifications Stratégiques de l'IA

## Introduction

Ce document détaille les améliorations successives apportées à l'intelligence artificielle des unités afin d'optimiser leurs stratégies et d'augmenter les chances de victoire. Les modifications couvrent la logique de soin, le ciblage et le mouvement.

## 1. Amélioration de la Stratégie de Soin (Drone Guérisseur)

### 1.1. Correction du Soin Non Fonctionnel

- **Problème :** Le drone guérisseur ne parvenait pas à soigner ses alliés car il ne trouvait pas leur composant `Health`.
- **Solution :** Modification du script `HealTarget.cs` pour rechercher le composant `Health` dans les objets enfants de la cible (`GetComponentInChildren`).
- **Impact :** Activation de la capacité de soin, augmentant drastiquement la durabilité de l'armée.

### 1.2. Optimisation des Déplacements

- **Problème :** Le drone était lent et sa trajectoire pour atteindre les alliés n'était pas directe, retardant le soin.
- **Solution :**
  1.  Augmentation de la vitesse de déplacement (`m_TranslationMaxSpeed`) dans l'arbre de comportement.
  2.  Modification du script `MyFlySeek.cs` pour assurer un déplacement en ligne droite vers la cible, tout en conservant une altitude constante par rapport au terrain.
  3.  Suppression de la phase de rotation à l'arrivée pour un déclenchement du soin plus rapide.
- **Impact :** Temps de réaction et de soin considérablement réduits.

### 1.3. Implémentation d'un Ciblage de Soin Intelligent

- **Problème :** Le choix de l'allié à soigner était aléatoire et donc sous-optimal.
- **Solution :** Remplacement de la logique aléatoire dans `FindWeakestAlly.cs` par un système de score pondéré. Le score de priorité est plus élevé pour les alliés ayant **peu de vie** et étant à **proximité**.
- **Impact :** Le soin est désormais dirigé vers les cibles les plus critiques, maximisant l'efficacité de chaque intervention.

## 2. Amélioration de la Stratégie Offensive (Drone d'Attaque)

### 2.1. Implémentation d'un Ciblage Offensif Intelligent

- **Problème :** Les drones attaquaient des cibles au hasard.
- **Solution :** Création du script `FindBestEnemyTarget.cs` qui utilise un score pondéré (points de vie bas + proximité) pour choisir la cible ennemie la plus pertinente.
- **Impact :** Concentration des tirs sur les cibles les plus faciles à éliminer, augmentant l'efficacité globale des dégâts.

### 2.2. Résolution du Problème de Groupement

- **Problème :** Tous les drones choisissaient la même "meilleure" cible et se regroupaient de manière inefficace.
- **Solution :** Modification de `FindBestEnemyTarget.cs` pour que chaque drone choisisse une cible au hasard parmi les **3 meilleures cibles disponibles**, au lieu de systématiquement prendre la meilleure.
- **Impact :** Meilleure répartition de la force de frappe, réduction du surplus de dégâts ("overkill") et couverture d'une plus grande zone de combat.

### 2.3. Priorisation Stratégique des Cibles (Tourelles)

- **Problème :** Les tourelles ennemies, avec leurs dégâts de zone, représentaient une menace prioritaire non gérée.
- **Solution :**
  1.  Création d'un script dédié, `FindBestTurretTarget.cs`, pour cibler exclusivement les tourelles.
  2.  Mise en place d'une structure dans l'arbre de comportement (`Selector`) qui force les drones à attaquer les tourelles en premier et à ne passer aux autres ennemis qu'une fois toutes les tourelles détruites.
- **Impact :** Mise en place d'une véritable stratégie en deux phases, neutralisant d'abord la plus grande menace.

### 2.4. Amélioration de la Poursuite et de l'Attaque

- **Problème 1 (Changement de cible) :** Les drones pouvaient changer de cible en plein combat si une "meilleure" cible apparaissait.
- **Solution :** Création d'une condition `HasTarget.cs` et modification de l'arbre pour forcer le drone à conserver sa cible actuelle tant qu'elle est en vie.
- **Problème 2 (Poursuite inefficace) :** La séquence "Aller vers -> Tourner -> Tirer" était trop lente contre des cibles mobiles.
- **Solution :** Création de la tâche optimisée `SeekAndFaceTarget.cs` qui fusionne le déplacement et la rotation en une seule action fluide et continue.
- **Impact :** Concentration des tirs jusqu'à la destruction de la cible et efficacité de poursuite drastiquement améliorée contre les drones ennemis mobiles.

## Conclusion

L'ensemble de ces modifications a transformé une IA basique en une force de frappe coordonnée et stratégique. Les unités choisissent désormais leurs cibles de manière intelligente, priorisent les menaces, et poursuivent leurs ennemis efficacement, ce qui augmente considérablement les probabilités de remporter la bataille.
