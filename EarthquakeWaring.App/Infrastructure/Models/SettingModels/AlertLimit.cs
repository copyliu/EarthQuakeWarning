﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EarthquakeWaring.App.Infrastructure.ServiceAbstraction;

namespace EarthquakeWaring.App.Infrastructure.Models.SettingModels;

public class AlertLimit : INotificationOption
{
    private double _dayMagnitude = 3.0;
    private double _dayIntensity = 2.0;
    private double _nightMagnitude;
    private double _nightIntensity;

    public double DayMagnitude
    {
        get => _dayMagnitude;
        set => SetField(ref _dayMagnitude, value);
    }

    public double DayIntensity
    {
        get => _dayIntensity;
        set => SetField(ref _dayIntensity, value);
    }

    public double NightMagnitude
    {
        get => _nightMagnitude;
        set => SetField(ref _nightMagnitude, value);
    }

    public double NightIntensity
    {
        get => _nightIntensity;
        set => SetField(ref _nightIntensity, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}