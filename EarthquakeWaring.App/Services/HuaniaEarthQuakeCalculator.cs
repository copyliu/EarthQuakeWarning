﻿using System;
using EarthquakeWaring.App.Extensions;
using EarthquakeWaring.App.Infrastructure;
using EarthquakeWaring.App.Infrastructure.ServiceAbstraction;
using Microsoft.Extensions.Logging;

namespace EarthquakeWaring.App.Services;

public class HuaniaEarthQuakeCalculator : IEarthQuakeCalculator
{
    private const double EARTH_RADIUS = 6378.137d;

    private static double[] SLOPE =
        { 0.23335281, 0.23347212, 0.23335606, 0.23335613, 0.23335539, 0.23335367, 0.23335291f };

    private static double[] INTERCEPT =
        { 8.567052, 7.5333714, 6.667651, 8.562906, 7.877903, 7.191011, 6.5055184f };

    private static double[][] ARRAY =
    {
        new[]
        {
            0.0, 22.264, 44.527, 66.79, 89.053, 111.314, 133.573, 155.832, 178.088, 200.342, 222.594,
            244.842, 267.088, 289.331, 311.57, 333.806, 356.037, 378.264, 400.486, 422.704, 444.916, 467.123,
            489.324, 511.519, 533.708, 555.891, 578.066, 600.235, 622.396, 644.55, 666.696, 688.834, 710.963,
            733.084, 755.196, 777.299, 799.392, 821.475, 843.548, 865.612, 887.664, 909.706, 931.737,
            953.756, 975.764, 997.759, 1019.743, 1041.714, 1063.673, 1085.618, 1107.551, 1129.469, 1151.375,
            1173.266, 1195.142, 1217.005, 1238.852, 1260.684, 1282.501, 1304.302, 1326.088, 1347.857, 1369.61,
            1391.346, 1413.065, 1434.767, 1456.452, 1478.118, 1499.767, 1521.397, 1543.009, 1564.602,
            1586.176, 1607.731, 1629.266, 1650.782, 1672.277, 1693.751, 1715.206, 1736.639, 1758.051,
            1779.442, 1800.811, 1822.158, 1843.482, 1864.785, 1886.064, 1907.321, 1928.554, 1949.764,
            1970.951, 1992.113, 2013.251, 2034.364, 2055.452, 2076.516, 2097.554, 2118.567, 2139.554,
            2160.514, 2181.449f
        },
        new[]
        {
            0.0, 6.619, 13.238, 19.857, 26.476, 33.095, 39.714, 46.333, 51.444, 56.545, 61.646, 66.747,
            71.848, 76.949, 82.05, 87.15, 92.251, 97.352, 102.453, 107.554, 112.655, 117.756, 122.857,
            127.957, 133.058, 138.159, 143.26, 148.361, 153.462, 158.563, 163.663, 168.764, 173.865, 178.966,
            184.067, 189.168, 194.269, 199.37, 204.47, 209.571, 214.672, 219.773, 224.874, 229.975, 235.076,
            240.176, 245.277, 250.378, 255.479, 260.58, 265.681, 270.782, 275.882, 280.983, 286.084, 291.185,
            296.286, 301.387, 306.488, 311.589, 316.689, 321.79, 326.891, 331.992, 337.093, 342.194, 347.295,
            352.396, 357.496, 362.597, 367.698, 372.799, 377.9, 383.001, 388.102, 393.202, 398.303, 403.404,
            408.505, 413.606, 418.707, 423.808, 428.909, 434.009, 439.11, 444.211, 449.312, 454.413, 459.514,
            464.615, 469.716, 474.816, 479.917, 485.018, 490.119, 495.22, 500.321, 505.422, 510.523, 515.624,
            520.725f
        },
        new[]
        {
            1.488, 6.807, 13.346, 19.911, 26.515, 33.11, 39.745, 45.417, 50.488, 55.613, 60.684, 65.8,
            70.917, 76.021, 81.095, 86.182, 91.322, 96.399, 101.49, 106.582, 111.681, 116.818, 121.929,
            126.99, 132.1, 137.192, 142.301, 147.404, 152.503, 157.606, 162.737, 167.834, 172.9, 178.001,
            183.117, 188.208, 193.3, 198.412, 203.502, 208.624, 213.713, 218.84, 223.942, 229.021, 234.134,
            239.244, 244.333, 249.427, 254.511, 259.628, 264.712, 269.821, 274.944, 280.043, 285.113,
            290.226, 295.319, 300.438, 305.562, 310.665, 315.765, 320.832, 325.938, 331.029, 336.156,
            341.256, 346.37, 351.454, 356.525, 361.647, 366.738, 371.866, 376.964, 382.067, 387.136, 392.269,
            397.362, 402.467, 407.557, 412.675, 417.757, 422.883, 427.936, 433.063, 438.138, 443.259, 448.36,
            453.482, 458.554, 463.648, 468.78, 473.872, 478.945, 484.079, 489.15, 494.257, 500.321, 505.422,
            510.523, 515.624, 520.725f
        },
        new[]
        {
            2.976, 7.259, 13.567, 20.069, 26.647, 33.213, 39.344, 44.424, 49.543, 54.634, 59.775, 64.86,
            69.955, 75.037, 80.128, 85.261, 90.342, 95.439, 100.544, 105.649, 110.746, 115.884, 120.963,
            126.043, 131.172, 136.266, 141.386, 146.47, 151.554, 156.656, 161.748, 166.873, 171.985, 177.062,
            182.191, 187.259, 192.364, 197.497, 202.568, 207.661, 212.771, 217.892, 222.952, 228.06, 233.166,
            238.263, 243.393, 248.495, 253.575, 258.688, 263.798, 268.864, 273.986, 279.094, 284.164,
            289.287, 294.388, 299.51, 304.603, 309.694, 314.779, 319.886, 325.016, 330.118, 335.206, 340.312,
            345.408, 350.504, 355.625, 360.721, 365.82, 370.915, 376.025, 381.111, 386.203, 391.311, 396.402,
            401.523, 406.604, 411.716, 416.814, 421.893, 427.016, 432.121, 437.217, 442.3, 447.421, 452.517,
            457.61, 462.705, 467.796, 472.935, 478.007, 483.127, 488.235, 493.307, 498.446, 503.519, 508.616,
            513.739, 518.848f
        },
        new[]
        {
            4.464, 7.978, 13.939, 20.351, 26.823, 33.387, 39.971, 46.348, 51.456, 56.542, 61.646, 66.762,
            71.841, 76.933, 82.047, 87.147, 92.235, 97.359, 102.459, 107.537, 112.663, 117.759, 122.861,
            127.945, 133.04, 138.142, 143.259, 148.341, 153.47, 158.54, 163.654, 168.788, 173.86, 178.971,
            184.047, 189.159, 194.289, 199.348, 204.467, 209.595, 214.678, 219.796, 224.858, 229.963,
            235.055, 240.158, 245.26, 250.393, 255.494, 260.602, 265.697, 270.77, 275.865, 280.979, 286.091,
            291.196, 296.281, 301.402, 306.487, 311.594, 316.679, 321.807, 326.899, 331.974, 337.077,
            342.209, 347.299, 352.403, 357.497, 362.59, 367.694, 372.785, 377.919, 382.976, 388.104, 393.188,
            398.307, 403.425, 408.527, 413.611, 418.688, 423.805, 428.92, 434.001, 439.118, 444.203, 449.331,
            454.404, 459.512, 464.596, 469.716, 474.804, 479.921, 485.044, 490.136, 495.232, 500.339,
            505.444, 510.501, 515.627, 520.723f
        },
        new[]
        {
            5.801, 8.719, 14.355, 20.592, 27.027, 33.577, 40.088, 45.633, 50.735, 55.874, 60.958, 66.056,
            71.175, 76.238, 81.356, 86.451, 91.545, 96.677, 101.776, 106.861, 111.959, 117.053, 122.182,
            127.277, 132.358, 137.465, 142.576, 147.671, 152.797, 157.879, 162.983, 168.072, 173.199,
            178.289, 183.402, 188.458, 193.593, 198.668, 203.778, 208.906, 213.961, 219.093, 224.179,
            229.275, 234.363, 239.47, 244.581, 249.679, 254.802, 259.878, 265.009, 270.093, 275.203, 280.304,
            285.409, 290.513, 295.596, 300.688, 305.817, 310.901, 315.995, 321.105, 326.216, 331.322,
            336.409, 341.53, 346.613, 351.706, 356.801, 361.914, 367.015, 372.132, 377.223, 382.335, 387.417,
            392.51, 397.608, 402.718, 407.824, 412.931, 418.029, 423.111, 428.244, 433.316, 438.437, 443.516,
            448.631, 453.748, 458.812, 463.938, 469.022, 474.119, 479.24, 484.321, 489.419, 494.531, 499.638,
            504.737, 509.822, 514.944, 520.047f
        },
        new[]
        {
            7.138, 9.589, 14.811, 20.862, 27.254, 33.734, 39.859, 44.984, 50.063, 55.148, 60.277, 65.397,
            70.479, 75.593, 80.666, 85.772, 90.857, 95.98, 101.062, 106.197, 111.292, 116.378, 121.482,
            126.578, 131.684, 136.789, 141.889, 146.99, 152.107, 157.194, 162.278, 167.404, 172.494, 177.603,
            182.698, 187.8, 192.879, 197.994, 203.095, 208.187, 213.277, 218.388, 223.494, 228.579, 233.704,
            238.813, 243.884, 248.998, 254.086, 259.189, 264.295, 269.385, 274.508, 279.594, 284.694,
            289.786, 294.935, 299.997, 305.096, 310.226, 315.296, 320.427, 325.512, 330.6, 335.72, 340.821,
            345.939, 351.023, 356.113, 361.199, 366.33, 371.445, 376.536, 381.624, 386.74, 391.85, 396.929,
            402.02, 407.151, 412.231, 417.325, 422.422, 427.513, 432.653, 437.76, 442.832, 447.955, 453.027,
            458.129, 463.224, 468.338, 473.436, 478.559, 483.661, 488.749, 493.868, 498.952, 504.027,
            509.167, 514.241, 519.359f
        },
        new[]
        {
            8.475, 10.59, 15.353, 21.238, 27.517, 33.895, 39.203, 44.303, 49.377, 54.485, 59.586, 64.68,
            69.77, 74.896, 79.985, 85.081, 90.177, 95.275, 100.376, 105.489, 110.597, 115.697, 120.81,
            125.899, 131.022, 136.118, 141.173, 146.301, 151.4, 156.487, 161.586, 166.699, 171.792, 176.914,
            181.987, 187.127, 192.205, 197.301, 202.388, 207.502, 212.632, 217.719, 222.807, 227.919,
            232.995, 238.098, 243.221, 248.296, 253.402, 258.528, 263.606, 268.735, 273.841, 278.913,
            283.999, 289.118, 294.237, 299.331, 304.438, 309.539, 314.621, 319.727, 324.834, 329.95, 335.052,
            340.137, 345.242, 350.317, 355.446, 360.53, 365.656, 370.721, 375.816, 380.937, 386.029, 391.14,
            396.238, 401.34, 406.453, 411.565, 416.633, 421.769, 426.872, 431.96, 437.038, 442.14, 447.249,
            452.349, 457.469, 462.556, 467.64, 472.739, 477.865, 482.954, 488.063, 493.141, 498.281, 503.366,
            508.466, 513.553, 518.649f
        }
    };


    public double GetIntensity(double magnitude, double distance)
    {
        var log = (double)(((magnitude * 1.363d) + 2.941d) - (Math.Log(distance + 7.0d) * 1.494d));
        return log < 0.0f ? 0.0f : log;
    }

    public double GetCountDownSeconds(double depth, double distance)
    {
        if (depth < 0.0f || distance < 0.0f) {
            return 0.0f;
        }
        var i = (int) (depth / 5.0f);
        if (i > 6) {
            i = 6;
        }
        var fArr = ARRAY;
        var i2 = 0;
        var fArr2 = fArr[0];
        var fArr3 = fArr[i + 1];
        var length = fArr2.Length - 1;
        if (distance > fArr2[length]) {
            return (SLOPE[i] * distance) + INTERCEPT[i];
        }
        if (Math.Abs(distance - fArr2[length]) < 0.0) {
            return fArr3[^1];
        }
        while (i2 < length && distance >= fArr2[i2]) {
            i2++;
        }
        var i3 = i2 - 1;
        var i4 = i3 + 1;
        return fArr3[i3] + ((fArr3[i4] - fArr3[i3]) * ((distance - fArr2[i3]) / (fArr2[i4] - fArr2[i3])));

    }

    public double GetDistance(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        /*
         var deltaLatitude = (latitude2 - latitude1).ToRadians();
                  var deltaLongitude = (longitude2 - longitude1).ToRadians();
        var sin = (Math.Sin(deltaLatitude) * Math.Sin(deltaLatitude)) + (Math.Cos(latitude1.ToRadians()) * Math.Cos(latitude2.ToRadians()) * Math.Sin(deltaLongitude) * Math.Sin(deltaLongitude));
        return (double) (Math.Atan2(Math.Sqrt(sin), Math.Sqrt(1.0d - sin)) * 2.0d * EARTH_RADIUS);
        */
        var firstRadLat = (latitude1.ToRadians());
        var firstRadLng = (longitude1.ToRadians());
        var secondRadLat = (latitude2.ToRadians());
        var secondRadLng = (longitude2.ToRadians());


        var a = firstRadLat - secondRadLat;
        var b = firstRadLng - secondRadLng;
        var cal = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(firstRadLat)
            * Math.Cos(secondRadLat) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
        var result = Math.Round(cal * 10000) / 10000;
        return result;
    }
}