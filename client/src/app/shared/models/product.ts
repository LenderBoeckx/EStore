export interface Product {
    id: number;
    naam: string;
    beschrijving: string;
    prijs: number;
    fotoURL: string;
    type: string;
    merk: string;
    hoeveelheidInVoorraad: number;
}


export interface NewProduct {
    id?: number;
    naam: string;
    prijs: number;
    merk: string;
    type: string;
    hoeveelheidInVoorraad: number;
    beschrijving: string;
    fotoURL?: string;
    image?: File;
  }