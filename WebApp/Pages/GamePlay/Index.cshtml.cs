﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages.GamePlay
{
    public class Index : PageModel
    {
        
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;

        public Index(DAL.AppDbContext context,ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public GameOption GameOption { get; set; } = new GameOption();

        public BattleShip? BattleShip { get; set; } = null;

        public async Task OnGetAsync(string? selectMode)
        {
            GameOption = await _context.GameOptions
                .Include(g => g.Boats)
                .Include(g => g.GameSaveData).FirstOrDefaultAsync() ?? new GameOption();

            BattleShip ??= new BattleShip(GameOption);



            if (Request.Query["PlaceNormal"].ToString() == "Place Ships")
            {

                for (var i = 0; i < BattleShip.GetBoatArrays().Item1.Count(); i++)
                {
                    if (Request.Query.Count == 0) break;

                    var boatIndex = i + 1;
                    var x = int.Parse(Request.Query["p1_x_bIndex_" + boatIndex]);
                    var y = int.Parse(Request.Query["p1_y_bIndex_" + boatIndex]);
                    var horizontal = Request.Query["p1_horizontal_bIndex_" + boatIndex];

                    if (horizontal.Count == 1)
                    {
                        BattleShip.RotateBoat(i);
                    }

                    BattleShip.PlaceBoat(i, x, y);

                }

                BattleShip.ChangeWhoPlacesBoats();

                for (var i = 0; i < BattleShip.GetBoatArrays().Item2.Count(); i++)
                {
                    if (Request.Query.Count == 0) break;

                    var boatIndex = i + 1;
                    var x = int.Parse(Request.Query["p2_x_bIndex_" + boatIndex]);
                    var y = int.Parse(Request.Query["p2_y_bIndex_" + boatIndex]);
                    var horizontal = Request.Query["p2_horizontal_bIndex_" + boatIndex];

                    if (horizontal.Count == 1)
                    {
                        BattleShip.RotateBoat(i);
                    }

                    BattleShip.PlaceBoat(i, x, y);

                }

                BattleShip.UpdateBoatsOnBoard();
                var boatPlacementViolation =
                    RulesViolated(BattleShip.GetBoatArrays().Item2, BattleShip.GetBoards().Item2);
                if (boatPlacementViolation)
                {
                    Console.WriteLine("Rules Violated");
                    ModelState.AddModelError("", "Your boat is placement bad! for player B");
                }

                BattleShip.ChangeWhoPlacesBoats();
                BattleShip.UpdateBoatsOnBoard();

                boatPlacementViolation = RulesViolated(BattleShip.GetBoatArrays().Item1, BattleShip.GetBoards().Item1);
                if (boatPlacementViolation)
                {
                    Console.WriteLine("Rules Violated");
                    ModelState.AddModelError("", "Your boat placement is bad!  player A");
                }
            }

            if (Request.Query["PlaceAuto"].ToString() == "Place Automatically")
            {
                BattleShip.AutoShipSetupForOneBoard(BattleShip.GetBoatArrays().Item1);
                BattleShip.ChangeWhoPlacesBoats();
                BattleShip.AutoShipSetupForOneBoard(BattleShip.GetBoatArrays().Item2);
                Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            }

            CheckBoards();
        }

        private bool RulesViolated(GameBoat[] boatArray, CellState[,] boardArray)
        {
            return BattleShip!.CheckIfBoatsOverlap(boatArray) ||
                   BattleShip.CheckIfTouchViolated(boatArray, boardArray) ||
                   BattleShip.CheckIfBoatLimitIsViolated(boatArray);
        }

        private void CheckBoards()
        {
            for (var rowIndex = 0; rowIndex < BattleShip!.GetBoardHeight(); rowIndex++)
            {
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"| {BattleShip.GetBoards().Item1[rowIndex, colIndex]} |");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                
            }

            Console.WriteLine("======================================================");
            
            for (var rowIndex = 0; rowIndex < BattleShip!.GetBoardHeight(); rowIndex++)
            {
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"| {BattleShip.GetBoards().Item2[rowIndex, colIndex]} |");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                
            }

        }
    }
}