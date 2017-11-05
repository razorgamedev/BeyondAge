using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class Status : Component
    {
        public int Health { get; set; } = 3;

        public enum CharacterState {
            Healthy,
            Sick
        }

        public CharacterState State = CharacterState.Healthy;

        public Status()
        {

        }  
        
        public void Hurt(int ammount) => Health -= ammount;
        public void Heal(int ammount) => Health += ammount;

        public bool IsDead() => Health <= 0;
    }
}
