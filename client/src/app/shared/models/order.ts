
export interface Order {
    id: number;
    bestellingsDatum: string;
    koperEmail: string;
    leverAddress: LeveringsAdres;
    leveringsMethode: string;
    leveringsPrijs: number;
    betalingsOverzicht: BetalingsOverzicht;
    bestellingsItems: BestellingsItem[];
    subtotaal: number;
    bestellingsStatus: string;
    totaal: number;
    betalingsIntentId: string;
  }
  
  export interface LeveringsAdres {
    naam: string;
    straat: string;
    toevoeging?: string;
    plaats: string;
    provincie?: string;
    postcode: string;
    land: string;
  }
  
  export interface BetalingsOverzicht {
    last4: number;
    brand: string;
    expMonth: number;
    expYear: number;
  }
  
  export interface BestellingsItem {
    productId: number;
    productNaam: string;
    fotoUrl: string;
    prijs: number;
    hoeveelheid: number;
  }

  export interface OrderToCreate {
    winkelwagenId: string;
    leveringsMethodeId: number;
    leveringsAdres: LeveringsAdres;
    betalingsOverzicht: BetalingsOverzicht;
  }
  