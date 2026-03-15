# 🚗 MyGarage

> Application Windows Forms moderne pour gérer vos véhicules, suivre leurs entretiens et générer des rapports détaillés.

![Version](https://img.shields.io/badge/version-2.0.0-blue?style=for-the-badge)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey?style=for-the-badge&logo=windows)
![.NET](https://img.shields.io/badge/.NET-10.0-purple?style=for-the-badge&logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green?style=for-the-badge)

---

## ✨ Fonctionnalités

- 🚗 **Gestion des véhicules** — Ajout, suppression et consultation de vos véhicules
- 🔧 **Suivi des entretiens** — Enregistrement de chaque entretien avec date, type, kilométrage et coût
- 📋 **Historique détaillé** — Visualisation complète de l'historique d'un véhicule avec statistiques
- 📊 **Export Excel** — Génération de rapports Excel professionnels avec statistiques par véhicule
- 📧 **Envoi par email** — Envoi du rapport en pièce jointe via SMTP (Gmail supporté)
- 📱 **Notifications SMS** — Envoi de notifications via l'API Free Mobile (gratuit)
- 🔔 **Alertes entretien** — Vérification automatique en arrière-plan et alerte si une révision approche
- 🌙 **Dark / Light mode** — Thème sombre ou clair, switchable en un clic
- 🔄 **Mise à jour automatique** — Détection et téléchargement des nouvelles versions via GitHub Releases

---

## 📸 Aperçu

| Dark Mode | Light Mode |
|---|---|
| ![Dark](Assets/screenshot_dark.png) | ![Light](Assets/screenshot_light.png) |

---

## 🏗️ Architecture

Le projet suit une architecture en couches découplées :

```
MyGarage (WinForms UI)
    ↓
Service          — Orchestration, génération de rapports, notifications
    ↓
Manager          — Logique métier
    ↓
Repository       — Accès base de données SQLite
    ↓
Models           — Modèles de données
    ↓
BackgroundWorker — Service de vérification des entretiens
```

---

## 🚀 Installation

### Prérequis
- Windows 10 ou supérieur
- Aucune installation de .NET requise (application auto-contenue)

### Via le setup
1. Téléchargez `MyGarageSetup_vX.X.X.exe` depuis les [Releases](../../releases/latest)
2. Lancez l'installateur et suivez les instructions
3. Un raccourci est créé sur le Bureau et dans le menu Démarrer

### Base de données
La base de données SQLite est automatiquement créée au premier lancement dans :
```
C:\Users\<VotreNom>\Desktop\MyGarage\MyGarage.db
```

---

## ⚙️ Configuration

Créez un fichier `appsettings.json` dans le dossier d'installation en vous basant sur le modèle `appsettings.example.json` :

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "votre@gmail.com",
    "Password": "votre_mot_de_passe_application"
  },
  "FreeMobile": {
    "UserId": "votre_identifiant_free",
    "ApiKey": "votre_cle_api"
  }
}
```

> ⚠️ **Gmail** — Utilisez un [mot de passe d'application](https://myaccount.google.com/apppasswords) et non votre mot de passe habituel.

> ⚠️ **Free Mobile** — Activez les notifications SMS dans votre [espace client Free Mobile](https://mobile.free.fr) pour obtenir votre clé API.

---

## 🗄️ Structure de la base de données

```sql
-- Véhicules
CREATE TABLE Vehicles (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    marque         TEXT,
    modele         TEXT,
    kilometrage    INTEGER,
    annee          INTEGER,
    immatriculation TEXT
);

-- Entretiens
CREATE TABLE entretien (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    vehicle_id      INTEGER,
    date_entretien  TEXT,
    type_etretien   TEXT,
    kilometrage     INTEGER,
    cout            REAL,
    notes           TEXT,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(id)
);
```

---

## 🔄 Mise à jour automatique

Au démarrage, l'application vérifie automatiquement si une nouvelle version est disponible sur GitHub. Si c'est le cas, une fenêtre propose de télécharger et installer la mise à jour.

Pour publier une nouvelle version :
1. Mettez à jour la version dans `MyGarage.csproj`
```xml
<Version>X.X.X</Version>
<AssemblyVersion>X.X.X.0</AssemblyVersion>
```
2. Publiez l'application (**Build → Publish**)
3. Créez une nouvelle Release sur GitHub avec le tag `vX.X.X` et uploadez le `.exe`

---

## 🛠️ Développement

### Prérequis
- Visual Studio 2022 ou supérieur
- .NET 10 SDK
- SQLite

### Cloner et lancer
```bash
git clone https://github.com/Joris-Tazamoucht/MyGarage.git
cd MyGarage
```

Ouvrez `MyGarage.slnx` dans Visual Studio, puis :
1. Copiez `appsettings.example.json` → `appsettings.json` et renseignez vos identifiants
2. Lancez le projet `MyGarage` (F5)

### Packages NuGet utilisés
| Package | Usage |
|---|---|
| `Microsoft.Data.Sqlite` | Accès base de données SQLite |
| `ClosedXML` | Génération des rapports Excel |
| `Microsoft.Extensions.Hosting` | Service de fond (BackgroundWorker) |

---

## 📁 Structure du projet

```
MyGarage/
├── MyGarage/               # Projet principal WinForms (UI)
│   ├── Views/              # Formulaires
│   ├── Assets/             # Icônes et ressources
│   ├── AppTheme.cs         # Système de thème Dark/Light
│   ├── AppConfig.cs        # Lecture de appsettings.json
│   └── appsettings.json    # ⚠️ Non versionné (voir .gitignore)
├── MyGarageModels/         # Modèles de données
├── MyGarageManager/        # Logique métier
├── MyGarageRepository/     # Accès base de données
├── Service/                # Services (rapport, notification, update)
├── BackgroundWorker/       # Vérification des entretiens en arrière-plan
└── appsettings.example.json
```

---

## 📝 Licence

Ce projet est sous licence MIT — voir le fichier [LICENSE](LICENSE) pour plus de détails.

---

## 👤 Auteur

**Joris Tazamoucht**

[![GitHub](https://img.shields.io/badge/GitHub-Joris--Tazamoucht-black?style=flat-square&logo=github)](https://github.com/Joris-Tazamoucht)
