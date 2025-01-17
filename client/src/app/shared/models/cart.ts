import { nanoid } from 'nanoid';
import { Product } from './product';

export interface CartType {
    id: string;
    items: CartItem[];
}

export interface CartItem {
    productId: number;
    productNaam: string;
    prijs: number;
    hoeveelheid: number;
    fotoUrl: string;
    merk: string;
    type: string;
}

export class Cart implements CartType {
    id = nanoid();
    items: CartItem[] = [];
}