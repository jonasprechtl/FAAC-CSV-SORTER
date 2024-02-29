const fs = require('fs');
const readline = require('readline');

// Regex für das Extrahieren des Kennzeichens
const regex = /\b[A-ZÄÖÜ]{1,3}-[A-ZÄÖÜ]{1,2}-\d{1,4}[EH]?\b/g;

// Erstellen einer Instanz von readline
const rl = readline.createInterface({
  input: fs.createReadStream('./input/kennzeichen.csv'),
  output: process.stdout,
  terminal: false
});

// Erstellen von Schreib-Stream-Instanzen für die Ausgaben
const outputValid = fs.createWriteStream('./output/valid_kennzeichen.csv');
const outputInvalid = fs.createWriteStream('./output/invalid_kennzeichen.csv');

rl.on('line', (line) => {
  // Zerlegen der Zeile in Felder
  const fields = line.split(';');

  if (fields.length > 1) {
    // Extrahieren des Kennzeichens aus dem zweiten Feld
    const matches = fields[1].match(regex);
    if (matches) {
      // Ersetzen des gesamten Textes im zweiten Feld durch das extrahierte Kennzeichen
      fields[1] = matches.join(', ');
      // Schreiben der verarbeiteten Zeile in die Datei für gültige Kennzeichen
      outputValid.write(fields.join(';') + '\n');
    } else {
      // Schreiben der Zeile in die Datei für ungültige Kennzeichen
      outputInvalid.write(line + '\n');
    }
  } else {
    // Falls weniger als 2 Felder vorhanden sind, behandeln wir es als ungültig
    outputInvalid.write(line + '\n');
  }
});

rl.on('close', () => {
  console.log('Verarbeitung abgeschlossen.');
});
