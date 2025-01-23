import { nanoid } from 'nanoid';
import { Product } from './product';

export interface CartType {
    id: string;
    items: CartItem[];
    deliveryMethodId?: number;
    paymentIntentId?: string;
    clientSecret?: string;
    coupon?: Coupon
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

export interface Coupon {
    couponId: string,
    amountOff?: number,
    percentOff?: number,
    name: string,
    promotionCode: string
}

export class Cart implements CartType {
    id = nanoid();
    items: CartItem[] = [];
    deliveryMethodId?: number;
    paymentIntentId?: string;
    clientSecret?: string;
    coupon?: Coupon
}

