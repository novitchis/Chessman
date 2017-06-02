using ChessEngine;
using ChessEngineClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChessEngineClient.View
{
    public class ChessPieceTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            ChessPiece chessPiece = (ChessPiece)item;

            // the template names are ColorTypeTemplate
            string dataTemplateKey = String.Format("{0}{1}Template", chessPiece.Color, chessPiece.Type);

            return Application.Current.Resources[dataTemplateKey] as DataTemplate;
        }
    }
}
