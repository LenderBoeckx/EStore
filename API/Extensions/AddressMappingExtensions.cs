using System;
using API.DTOs;
using Core.Entities;

namespace API.Extensions;

public static class AddressMappingExtensions
{
    //waarden van entiteit Address omzetten naar AddressDto
    public static AddressDto? ToDto(this Address? address)
    {
        if(address == null) return null;

        return new AddressDto{
            Straat = address.Straat,
            Toevoeging = address.Toevoeging,
            Plaats = address.Plaats,
            Provincie = address.Provincie,
            Land = address.Land,
            Postcode = address.Postcode
        };
    }

    //waarden van AddressDto omzetten naar entiteit Address
    public static Address ToEntity(this AddressDto addressDto)
    {
        if(addressDto == null) throw new ArgumentNullException(nameof(addressDto));

        return new Address{
            Straat = addressDto.Straat,
            Toevoeging = addressDto.Toevoeging,
            Plaats = addressDto.Plaats,
            Provincie = addressDto.Provincie,
            Land = addressDto.Land,
            Postcode = addressDto.Postcode
        };
    }

    public static void UpdateFromDto(this Address address, AddressDto addressDto)
    {
        if(addressDto == null) throw new ArgumentNullException(nameof(addressDto));
        if(address == null) throw new ArgumentNullException(nameof(address));

        address.Straat = addressDto.Straat;
        address.Toevoeging = addressDto.Toevoeging;
        address.Plaats = addressDto.Plaats;
        address.Provincie = addressDto.Provincie;
        address.Land = addressDto.Land;
        address.Postcode = addressDto.Postcode;
    }
}
