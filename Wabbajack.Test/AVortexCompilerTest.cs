﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wabbajack.Common;
using Wabbajack.Lib;

namespace Wabbajack.Test
{
    public abstract class AVortexCompilerTest
    {
        public TestContext TestContext { get; set; }
        protected TestUtils utils { get; set; }


        [TestInitialize]
        public void TestInitialize()
        {
            Consts.TestMode = true;

            utils = new TestUtils
            {
                Game = Game.DarkestDungeon
            };

            Utils.LogMessages.Subscribe(f => TestContext.WriteLine(f));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            utils.Dispose();
        }

        protected VortexCompiler ConfigureAndRunCompiler()
        {
            var vortexCompiler = MakeCompiler();
            vortexCompiler.DownloadsFolder = utils.DownloadsFolder;
            vortexCompiler.StagingFolder = utils.InstallFolder;
            Directory.CreateDirectory(utils.InstallFolder);
            Assert.IsTrue(vortexCompiler.Begin().Result);
            return vortexCompiler;
        }

        protected VortexCompiler MakeCompiler()
        {
            return new VortexCompiler(
                game: utils.Game,
                gamePath: utils.GameFolder,
                vortexFolder: VortexCompiler.TypicalVortexFolder(),
                downloadsFolder: VortexCompiler.RetrieveDownloadLocation(utils.Game),
                stagingFolder: VortexCompiler.RetrieveStagingLocation(utils.Game));
        }

        protected ModList CompileAndInstall()
        {
            var vortexCompiler = ConfigureAndRunCompiler();
            Install(vortexCompiler);
            return vortexCompiler.ModList;
        }

        protected void Install(VortexCompiler vortexCompiler)
        {
            var modList = MO2Installer.LoadFromFile(vortexCompiler.ModListOutputFile);
            var installer = new MO2Installer(vortexCompiler.ModListOutputFile, modList, utils.InstallFolder)
            {
                DownloadFolder = utils.DownloadsFolder,
                GameFolder = utils.GameFolder,
            };
            installer.Begin().Wait();
        }
    }
}
