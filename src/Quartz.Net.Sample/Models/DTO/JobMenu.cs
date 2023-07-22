namespace Quartz.Net.Sample.Models.DTO;

internal class JobMenu
{
    public System.Type Type { get; set; }

    public string Description { get; set; }

    public int Sort { get; set; } = 0; // TODO: Use this property to sort the menu items in Interactive mode.

    public override string ToString()
    {
        const int delimiterMax = 60;
        string delimiter = ".....";
        if (this.Description.Length < delimiterMax)
            delimiter = "".PadRight(delimiterMax - this.Description.Length, '.');

        return $"{this.Description} {delimiter} {this.Type?.Name ?? "N/A"}";
    }
}
