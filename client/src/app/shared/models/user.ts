export interface User {
    firstName: string;
    lastName: string;
    email: string;
    address: Address;
    roles: string | string[];
}

export interface Address {
    straat: string;
    toevoeging?: string;
    plaats: string;
    provincie?: string;
    land: string;
    postcode: string;
}