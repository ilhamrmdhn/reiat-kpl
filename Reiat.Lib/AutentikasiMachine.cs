using System;

namespace Reiat.Lib
{
    public enum AuthState { Guest, Authenticating, Customer }

    // Teknik Automata: State Machine untuk login
    public class AutentikasiMachine
    {
        public AuthState StateSaatIni { get; private set; }

        public AutentikasiMachine()
        {
            StateSaatIni = AuthState.Guest; 
        }

        public void TriggerLogin()
        {
            // Defensive Programming 
            if (StateSaatIni != AuthState.Guest)
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

            StateSaatIni = AuthState.Customer;
        }
    }
}