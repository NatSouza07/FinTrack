document.addEventListener("DOMContentLoaded", function () {

    const moneyFields = document.querySelectorAll(".ft-money");

    moneyFields.forEach(field => {

        const inst = AutoNumeric.getAutoNumericElement(field);
        if (inst) inst.remove();

        new AutoNumeric(field, {
            digitGroupSeparator: ".",
            decimalCharacter: ",",
            decimalPlaces: 2,
            currencySymbol: "R$ ",
            currencySymbolPlacement: "p",
            unformatOnSubmit: true,
            modifyValueOnWheel: false
        });
    });
});