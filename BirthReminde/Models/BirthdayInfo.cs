using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BirthReminde.Models;

public partial class BirthdayInfo : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    
    public DateTime Date { get; set; } = DateTime.Now;
    
    public string? Notes { get; set; }

    public int GetDaysUntilBirthday()
    {
        var today = DateTime.Today;
        var nextBirthday = new DateTime(today.Year, Date.Month, Date.Day);
        
        if (nextBirthday < today)
        {
            nextBirthday = nextBirthday.AddYears(1);
        }
        
        return (nextBirthday - today).Days;
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - Date.Year;
        
        if (today.Month < Date.Month || (today.Month == Date.Month && today.Day < Date.Day))
        {
            age--;
        }
        
        return age;
    }
}