using System;
using System.Globalization;
using System.Windows.Data;

namespace MFAudioDeviceEnumeratorVorticeWpfApp
{
    public class FloatVolumeToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var floatVolume = (float)value;
            return $"{Math.Round(floatVolume * 100)}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}