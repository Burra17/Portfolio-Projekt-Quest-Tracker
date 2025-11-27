# 🛡️ Quest Tracker - Ditt Ultimata Äventyrsverktyg

Välkommen till **Quest Tracker**! Detta är en avancerad konsolapplikation skriven i C# designad för att gamifiera uppgiftshantering. Projektet kombinerar klassisk CRUD-funktionalitet med modern AI-hjälp och säker inloggning via SMS.

Detta är mitt första större C#-projekt och syftar till att utforska objektorienterad programmering, asynkrona anrop och API-integrationer.

## 🚀 Funktioner

* **Quest Management:** Skapa, läs, uppdatera och slutför dina uppgifter (Quests).
* **🤖 Guild Advisor (AI):** En integrerad AI-assistent som ger råd och tips baserat på dina quests.
* **🔐 Säkerhet:** Inloggningssystem med 2-faktorsautentisering (2FA) via SMS för extra säkerhet.
* **Snyggt UI:** Ett användarvänligt gränssnitt i terminalen byggt med *Spectre.Console*.
* **Data Persistence:** Dina quests och användardata sparas lokalt så att inget går förlorat.

## 🛠️ Tekniker

* **Språk:** C# (.NET 8.0)
* **Ramverk/Bibliotek:**
    * Spectre.Console (för UI)
    * OpenAI API (för AI-rådgivaren)
    * Twilio (för 2FA)
    * System.Text.Json (för datalagring)

## ⚙️ Förutsättningar

För att köra detta program behöver du:
1.  **.NET SDK** installerat på din dator.
2.  En API-nyckel för AI-tjänsten (OpenAI).
3.  Kontouppgifter för SMS-tjänsten (Twilio).

## 📥 Installation och Konfiguration

För att garantera säkerheten använder detta projekt **Miljövariabler** för att hantera API-nycklar. Inga nycklar sparas direkt i koden.

1.  **Klona repot:**
    *(Byt ut länken nedan till din egen GitHub-länk)*
    ```bash
    git clone [https://github.com/DittAnvändarnamn/Quest-Tracker.git](https://github.com/DittAnvändarnamn/Quest-Tracker.git)
    cd Quest-Tracker
    ```

2.  **Ställ in Miljövariabler:**
    Du behöver konfigurera följande variabler i din utvecklingsmiljö eller ditt operativsystem:

    **För AI-assistenten (OpenAI):**
    * `OPENAI_API_KEY` - Din personliga API-nyckel från OpenAI.

    **För 2-faktorsautentisering (Twilio):**
    * `TWILIO_ACCOUNT_SID` - Ditt Account SID.
    * `TWILIO_AUTH_TOKEN` - Din Auth Token.
    * `TWILIO_FROM_NUMBER` - Ditt Twilio-telefonnummer.

3.  **Installera paket:**
    Kör följande kommando för att hämta nödvändiga NuGet-paket:
    ```bash
    dotnet restore
    ```

## ▶️ Hur man kör programmet

Du kan köra programmet direkt från din terminal eller via Visual Studio.

**Via Terminalen:**
Stå i projektmappen och kör:
```bash
dotnet run
