# Furniture Factory Manager

Jednostavna ASP.NET Core MVC aplikacija za organizaciju projekata i zadataka u fabrici nameštaja. Aplikacija prikazuje modernu admin tablu sa tri kolone (To do, U toku, Završeno), omogućava prevlačenje zadataka između kolona, izbor aktivnog projekta i unos novih zadataka bez složenih obrazaca ili dodatnih biblioteka.

## Pokretanje

1. Instalirajte [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download).
2. U terminalu pozicioniranom u korenskom folderu repozitorijuma pokrenite:
   ```bash
   dotnet run --project FurnitureFactoryManager/FurnitureFactoryManager.csproj
   ```
3. Otvorite prikazanu adresu (podrazumevano `https://localhost:7121` ili `http://localhost:5121`).
4. Sa leve strane birajte projekat, dodajte novi zadatak i organizujte rad prevlačenjem između kolona.

## Pokretanje kao desktop aplikacija

Aplikaciju možete objaviti kao samostalnu aplikaciju koja se startuje dvostrukim klikom:

```bash
dotnet publish FurnitureFactoryManager/FurnitureFactoryManager.csproj -c Release -r win-x64 --self-contained true
```

U rezultujućem `publish` folderu nalazi se izvršni fajl koji se može pokrenuti bez dodatne instalacije.

## Deploy na web server

Bez dodatnih izmena možete objaviti aplikaciju na klasičan web server koristeći `dotnet publish` komandu i kopirati sadržaj `publish` foldera na server. Aplikacija ne koristi bazu podataka niti eksterne servise pa je migracija jednostavna.

## Povezivanje sa Git repozitorijumom

Ukoliko želite da kod bude dostupan na udaljenom repozitorijumu (npr. GitHub), potrebno je da dodate `origin` adresu i pošaljete promene:

```bash
git remote add origin https://github.com/<korisnicko-ime>/<repo>.git
git branch -M main
git push -u origin main
```

Nakon inicijalnog objavljivanja, nove izmene šaljete komandom `git push`.
