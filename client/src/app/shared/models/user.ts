export interface User {
    firstName: string;
    lastName: string;
    email: string;
    address: Address;
}

export interface Address {
    straat: string;
    toevoeging: string;
    plaats: string;
    provincie: string;
    land: string;
    postcode: string;
}