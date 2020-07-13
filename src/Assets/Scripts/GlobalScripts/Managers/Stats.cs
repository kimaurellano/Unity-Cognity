using System;
using Assets.Scripts.DataComponent.Model;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Managers {
    public class Stats : MonoBehaviour {
        public event EventHandler OnStatsChanged;

        public static int STAT_MIN = 0;

        public static int STAT_MAX = 20;

        private SingleStat flexibility;

        private SingleStat language;

        private SingleStat memory;

        private SingleStat problemSolving;

        private SingleStat speed;

        public Stats(int flexibility, int language, int memory, int problemSolving, int speed) {
            this.flexibility = new SingleStat(flexibility);
            this.language = new SingleStat(language);
            this.memory = new SingleStat(memory);
            this.problemSolving = new SingleStat(problemSolving);
            this.speed = new SingleStat(speed);
        }

        private SingleStat GetSingleStat(UserStat.GameCategory statType) {
            switch (statType) {
                default:
                case UserStat.GameCategory.Flexibility:
                    return flexibility;
                case UserStat.GameCategory.Language:
                    return language;
                case UserStat.GameCategory.Memory:
                    return memory;
                case UserStat.GameCategory.ProblemSolving:
                    return problemSolving;
                case UserStat.GameCategory.Speed:
                    return speed;
            }
        }

        public void SetStatAmount(UserStat.GameCategory statType, int statAmount) {
            GetSingleStat(statType).SetStatAmount(statAmount);
            OnStatsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void IncreaseStatAmount(UserStat.GameCategory statType) {
            SetStatAmount(statType, GetStatAmount(statType) + 1);
        }

        public void DecreaseStatAmount(UserStat.GameCategory statType) {
            SetStatAmount(statType, GetStatAmount(statType) - 1);
        }

        public int GetStatAmount(UserStat.GameCategory statType) {
            return GetSingleStat(statType).GetStatAmount();
        }

        public float GetStatAmountNormalized(UserStat.GameCategory statType) {
            return GetSingleStat(statType).GetStatAmountNormalized();
        }

        /*
     * Represents a Single Stat of any Type
     * */
        private class SingleStat {

            private int stat;

            public SingleStat(int statAmount) {
                SetStatAmount(statAmount);
            }

            public void SetStatAmount(int statAmount) {
                stat = Mathf.Clamp(statAmount, STAT_MIN, STAT_MAX);
            }

            public int GetStatAmount() {
                return stat;
            }

            public float GetStatAmountNormalized() {
                return (float) stat / STAT_MAX;
            }
        }
    }
}
