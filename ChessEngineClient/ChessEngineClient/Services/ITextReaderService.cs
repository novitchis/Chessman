﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ChessEngineClient.Services
{
    public interface ITextReaderService
    {
        Task<string> ReadText(IStorageFile file);
    }
}
