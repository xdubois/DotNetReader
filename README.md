DotNetReader
============

Basic C# SyndicationFeed Reader

Compte par défaut
-----------------

Compte d'administration: administrator - administrator



Quels types de flux sont supportés par l'application ?
--------------------------------------------------------

Actuellement, seulement les flux valide RSS 2.0 et Atom 1.0


Qu'est-ce que le type de synchronisation ?
-------------------------


| Méthode / Synchro |Màj via clique du bouton Update    | Màj Distante (Cron ou tâche)| Màj  via clique d’un flux rss
| ------------- | -----:| -----:| -----:|
| Fullx         |   x   |   x   |       |
| Partial       |               |       |   x   |
| Manual        |   x   |       |       |

Appel Cron 
-------------------------
http://yourServer/Feed/UpdateAll/YourSynchroCode


Quelles sont les modes de Display ?
----------

Spécifie le mode de visualisation des articles.

* Only unread items //Affiche que les articles qui n'ont pas été lu
* All items //Affiche tous les articles

Qu'est-ce que la Cache limit ?
--------------

Valeur entre 1 et 100


Définit le nombre d'articles que l'application doit garder en cache

/!\ Plus le nombre est élevé plus le temps de synchronisation sera lent


