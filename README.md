# draka-o-hajs
## Wytyczne Projektu

### 1. **Struktura Plików w Projekcie Unity**

1. **Folder `Assets/`**
   - Główna lokalizacja wszystkich zasobów i skryptów. Proszę o trzymanie schematu przedstawionego poniżej:
     ```
     Assets/
     ├── _ProjectName/            # Główny folder projektu
     │   ├── Art/                 # Modele 3D, tekstury, materiały
     │   ├── Audio/               # Pliki dźwiękowe
     │   ├── Code/                # Cały kod C# (poza UI)
     │   ├── Prefabs/             # Prefaby (poza UI)
     │   ├── Scenes/              # Sceny
     │   └── UI/                  # Elementy interfejsu użytkownika (Prefaby i skrypty)
     └── **/                      # Pozostałe Biblioteki
     ```

2. **Folder `Code/`**
   - W folderze `Code/` również powinien panować porządek. Zaleca się organizowanie skryptów według funkcjonalności. Przykład:
     ```
     Code/
     ├── Models/                  # Klasy, Enum'y, Struct'y etc.
     ├── Managers/                # Zarządcy (GameManager, AudioManager, itp.)
     ├── Scripts/                 # Skrypty do obiektów
     └── Utilities/               # Skrypty/funkcje pomocnicze (np. dzielone pomiędzy obiektami)
     ```

3. **Prefaby, Skrypty**
   - Prefaby i skrypty powinny być organizowane według kategorii, np. `UI`, `Environment`, `Characters`, itp.

### 2. **Nazywanie Zmiennych i Metod**

1. **Zasady Ogólne**
   - **Zmienne prywatne**: camelCase z przedrostkiem `_`, np. `_playerHealth`.
   - **Zmienne publiczne**: PascalCase, np. `PlayerHealth`.
   - **Stałe**: Wszystkie litery wielkie, oddzielone podkreśleniami, np. `MAX_HEALTH`. Zmienne te trzymamy w pliku `constants.cs` w folderze `Code/Models`
   - **Metody**: PascalCase, np. `MovePlayer()`, `GetScore()`.
   - **Właściwości**: PascalCase, np. `PlayerHealth { get; set; }`.

2. **Konwencje Nazywania**
   - Zmienna powinna jasno komunikować swoją funkcję i typ. Zasada KISS (Keep It Simple, Stupid): np. zamiast `a` nazywaj zmienne opisowo, np. `playerScore`.
   - W przypadku potrzeby stworzenia wielu zmiennych o takich samych nazwach, należy dokładniej opisać funkcję zmiennej, np. `playerNameText`, `playerNameSize`.
   - Unikaj skrótów, chyba że są powszechnie zrozumiałe, np. `UI` (User Interface), `s` (Sekundy).

### 3. **Gałęzie i Pull Request'y**

1. **Gałęzie"**
   - Nazwy powinny być krótkie, lecz opisowe. Zaleca się stosowanie formatu:
     ```
     feature/{opis-funkcji}        # Przykład: feature/black-box
     fix/{opis-poprawki}           # Przykład: fix/ui-button-alignment
     hotfix/{opis-pilnej-poprawki} # Przykład: hotfix/crash-on-start
     ```

   **Zalecenia:**
   - Wszystkie litery w nazwach branchy powinny być małe, a słowa oddzielone myślnikami.
   - Każda gałąź powinna być tworzona z głównej gałęzi projektu (`develop`). Gałąź `main` zarezerwowana jest tylko do *stabilnych* (przetestowanych) wersji gry.

3. **Pull Request'y:**
   - Powinny być przypisane do konkretnego issue.
   - Powinny być nazwane zgodnie z ich treścią, np.:
     - `[Feature] Implementacja ruchu postaci`
     - `[Fix] Poprawa wyświetlania UI na małych ekranach`

   **Zawartość Pull Request'ów**:
   - Krótki opis zmian.
   - Czy PR wymaga dodatkowych działań przed scaleniem, np. stworzenia scenariusza testowego, review kodu itp.

### 4. **Komentarze w Kodzie**

1. **Komentarze Powinny Tłumaczyć "Dlaczego", a Nie "Co"**
   Komentarze powinny tłumaczyć **cel** danego fragmentu kodu, a nie to, co on robi (co powinno być zrozumiałe z samego kodu).

2. **Komentowanie Dłuższych Fragmentów Kodów**
   - Jeśli fragment kodu jest długi i złożony, zaleca się rozbicie logiki na funkcje lub wstawianie komentarzy wyjaśniających logikę.

### 5. **Formatowanie Kodu**

1. **Puste Linie**
   - Stosuj puste linie do oddzielania logicznych sekcji kodu.

### 6. **Pushowanie kodu**

1. **Commity**
   - Każdy commit powinien być opisany zwięzłym, ale informacyjnym tytułem.
   - Komentarze commitów powinny być w formie trybu rozkazującego, np.:
     ```
     Add character movement logic
     Fix audio manager initialization bug
     ```

### 7. **Testy i Code Review**

1. **Testy**
   Każdy pull request powinien zostać sprawdzony i zaakceptowany przez przynajmniej 2 testerów przed merge'em. W przypadku znalezienia błędów, tester zobowiązany napisać jako komentarz, jak doszedł do błędu.

2. **Code Review**
   Każdy pull request po sprawdzeniu przez grupe testerską, powinien również zostać zaakceptowany przez przynajmniej jedną z autoryzowanych do tego deweloperów. Zaleca się sprawdzenie:
   - Czy kod jest zgodny z ustalonym stylem.
   - Czy nie zawiera zbędnych elementów.
   - Czy jest odpowiednio udokumentowany.
   Deweloper zobowiązany jest poprawić wedle wskazówek swój kod lub przekonać leadera deweloperów o zamianę taska z innym deweloperem.

---

Te zasady mają na celu ułatwienie pracy zespołowej oraz utrzymanie porządku w projekcie, co przekłada się na większą produktywność i czytelność kodu. Powodzenia <3
