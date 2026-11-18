# Gmaili seadistamine

Arve saatmiseks e-posti teel kasutab rakendus Gmaili SMTP-serverit.

Enne e-kirjade saatmist tuleb Windowsis seadistada kaks keskkonnamuutujat:

- `GMAIL_ADDRESS` – Gmaili e-posti aadress, millelt arve saadetakse.
- `GMAIL_APP_PASSWORD` – Gmaili rakenduse parool.

## Keskkonnamuutujate lisamine Windowsis

1. Ava Windowsi Start-menüü.
2. Otsi „Muuda süsteemi keskkonnamuutujaid“.
3. Ava „Keskkonnamuutujad“.
4. Vali kasutajamuutujate alt „Uus“.
5. Lisa muutuja `GMAIL_ADDRESS` ja sisesta oma Gmaili aadress.
6. Lisa teine muutuja `GMAIL_APP_PASSWORD` ja sisesta Gmaili rakenduse parool.
7. Salvesta muudatused.

Pärast muutujate lisamist taaskäivita rakendus.

## Kui keskkonnamuutujad puuduvad

Kui üks või mõlemad keskkonnamuutujad puuduvad või on tühjad, kuvab rakendus teate:

`Gmaili seaded puuduvad!`

Sellisel juhul e-kirja ei saadeta.

**Turvalisus:** Gmaili parooli ega rakenduse parooli ei tohi lisada projekti lähtekoodi ega GitHubi. Iga kasutaja peab seadistama oma keskkonnamuutujad eraldi.