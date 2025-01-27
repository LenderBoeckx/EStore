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

export type NewProduct = Omit<Product, 'id'>;