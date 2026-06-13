using System;

namespace Reiat.Lib
{
    public enum AuthState { Unauthenticated, Authenticating, Authenticated }

    // Teknik Automata: State Machine untuk login
    public class AutentikasiMachine
    {
        public AuthState StateSaatIni { get; private set; }

        public AutentikasiMachine()
        {
            // State awal disesuaikan
            StateSaatIni = AuthState.Unauthenticated;
        }

        public void TriggerLogin()
        {
            // Defensive Programming 
            if (StateSaatIni != AuthState.Unauthenticated)
            {
                throw new InvalidOperationException("User sudah login atau sedang dalam proses login.");
            }

            StateSaatIni = AuthState.Authenticating;
        }

        public void SuksesLogin()
        {
            if (StateSaatIni != AuthState.Authenticating)
            {
                throw new InvalidOperationException("Harus melewati tahap autentikasi (input email/password) terlebih dahulu.");
            }

            // State akhir disesuaikan
            StateSaatIni = AuthState.Authenticated;
        }
    }
}