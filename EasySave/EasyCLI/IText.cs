using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCli;
public interface IText
{
    string this[string key] { get; }
}
/* Pour simplifier l'utilisation des deux langues ; on a une clé lié à un texte, défini dans l'appli.
 * Ex : si Fr ["help.menu"] = "Menu aide" sinon ["help.menu"] = "Help menu"
 * ça permet d'éviter de dupliquer les appels
 */