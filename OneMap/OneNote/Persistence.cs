using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMap.OneNote
{
    public interface IPersistence
    {
        /// <summary>
        /// Get the currently available notebooks
        /// </summary>
        /// <returns></returns>
        Notebooks LoadNotebooks();

        Page GetPage(string pageId);


        Page GetCurrentPage();


        void UpdatePage(Page page);
    }

    public class OneNotePersistence : IPersistence
    {

    }
}
