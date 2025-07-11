﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCore.Harmony.PlayerFeatures
{
    public class SCoreEntityStats
    {
        [HarmonyPatch(typeof(EntityStats), nameof(EntityStats.Tick))]
        public class EntityStats_UpdateWeatherStats
        {
            private static void Postfix(EntityStats __instance)
            {
                var rain = WeatherManager.Instance.GetCurrentRainfallValue();
                var snow = WeatherManager.Instance.GetCurrentSnowfallValue();
                var cloud = WeatherManager.Instance.GetCurrentCloudThicknessPercent();
                __instance.m_entity.Buffs.SetCustomVar("_sc_rain", rain, true);
                __instance.m_entity.Buffs.SetCustomVar("_sc_snow", snow, true);
                __instance.m_entity.Buffs.SetCustomVar("_sc_cloud", cloud, true);
            }
        }
    }
}
