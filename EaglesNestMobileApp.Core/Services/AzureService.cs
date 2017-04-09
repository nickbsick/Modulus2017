﻿/*****************************************************************************/
/*                               AzureService                                */
/*                                                                           */
/*                                                                           */
/*****************************************************************************/
using EaglesNestMobileApp.Core.Contracts;
using EaglesNestMobileApp.Core.Helpers;
using EaglesNestMobileApp.Core.Model;
using EaglesNestMobileApp.Core.Model.Campus;
using EaglesNestMobileApp.Core.Model.Home;
using EaglesNestMobileApp.Core.Model.Personal;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EaglesNestMobileApp.Core.Services
{
    public class AzureService : IAzureService
    {
        /* Private member variables                                          */
        private MobileServiceClient _client = App.Client;
        private MobileServiceSQLiteStore _eagleDatabase;
        private IMobileServiceSyncTable<Assignment> _assignmentTable;
        private IMobileServiceSyncTable<Course> _courseTable;
        private IMobileServiceSyncTable<EventsSignUp> _eventsSignUpTable;
        private IMobileServiceSyncTable<Offense> _offenseTable;
        private IMobileServiceSyncTable<OffenseCategory> _offenseCategoryTable;
        private IMobileServiceSyncTable<FourWindsItem> _fourWindsTable;
        private IMobileServiceSyncTable<VarsityItem> _varsityTable;
        private IMobileServiceSyncTable<GrabAndGoItem> _grabAndGoTable;
        private IMobileServiceSyncTable<LocalToken> _localTokenTable;
        private IMobileServiceSyncTable<AzureToken> _azureTokenTable;
        private IMobileServiceSyncTable<Student> _studentTable;
        private IMobileServiceSyncTable<ClassAttendance> _attendanceTable;
        private SyncHandler _syncHandler;
        private LocalToken _currentUser = new LocalToken();

        /*********************************************************************/
        /*   Initialize the database and specify locally persistent tables   */
        /*********************************************************************/
        public async Task InitLocalStore()
        {
            if (!_client.SyncContext.IsInitialized)
            {
                _eagleDatabase = new MobileServiceSQLiteStore(App.DatabaseName);

                /* Define all the tables                                     */
                DefineTables();

                /* Create the sync handler and specify tables to exclude     */
                _syncHandler = new SyncHandler();
                _syncHandler.Exclude<LocalToken>();

                /* Sync or something                                         */
                await _client.SyncContext.InitializeAsync(_eagleDatabase);

                /* Get references to the tables                              */
                GetReferences();

                //await SyncAsync(pullData:true);
            }
        }

        /*********************************************************************/
        /*                           Sync the data                           */
        /*********************************************************************/
        public async Task SyncAsync(bool pullData = false)
        {
            /* Local token is being used to make calls for "personal" data   */
            /* Try to sync the local store with the remote database          */
            _currentUser = await GetLocalTokenAsync();

            try
            {
                //await _client.SyncContext.PushAsync();
                if (pullData)
                {
                    /* Pull down student related tables                      */
                    await _assignmentTable.PullAsync("allAssignments",
                        _assignmentTable.Where(assignment =>
                            assignment.StudentId == _currentUser.Id));

                    await _courseTable.PullAsync("allCourses",
                        _courseTable.Where(course =>
                            course.StudentId == _currentUser.Id));


                    await _studentTable.PullAsync("currentStudent",
                        _studentTable.Where(student =>
                            student.Id == _currentUser.Id));
                    await _attendanceTable.PullAsync("AllAttendanceViolations",
                        _attendanceTable.Where(attendance => attendance.StudentId == _currentUser.Id));

                    await _offenseTable.PullAsync("AllStudentCourtOffenses",
                        _offenseTable.Where(offense => offense.StudentId == _currentUser.Id));

                    await _offenseCategoryTable.PullAsync("AllStudentCourtOffenseCategories",
                        _offenseCategoryTable.Where(offense => offense.StudentId == _currentUser.Id));

                    /******************************************************************************/
                    /**/
                    var whateverVariable = await _offenseTable.ToListAsync();          /**/
                                                                                       /**/
                    System.Diagnostics.Debug.WriteLine("\n\n\n" + whateverVariable.Count);/**/
                                                                                          /******************************************************************************/
                                                                                          /* Pull down non student related tables                 */
                    PullOptions data = new PullOptions { MaxPageSize = 150 };

                    await _fourWindsTable.PullAsync("allFourWindsItems",
                       _fourWindsTable.CreateQuery(), data);

                    await _varsityTable.PullAsync("allVarsityItems",
                        _varsityTable.CreateQuery());

                    await _grabAndGoTable.PullAsync("allGrabAndGoItems",
                        _grabAndGoTable.CreateQuery());

                    //await _eventsSignUpTable.PullAsync("AllEvents",
                    //    _eventsSignUpTable.CreateQuery());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} in SyncAsync");
                Debug.WriteLine("Please check your Internet connection!");
            }
        }

        /*********************************************************************/
        /*                          Define the Tables                        */
        /*********************************************************************/
        public void DefineTables()
        {
            _eagleDatabase.DefineTable<Assignment>();
            _eagleDatabase.DefineTable<Course>();
            _eagleDatabase.DefineTable<FourWindsItem>();
            _eagleDatabase.DefineTable<VarsityItem>();
            _eagleDatabase.DefineTable<GrabAndGoItem>();
            _eagleDatabase.DefineTable<Student>();
            _eagleDatabase.DefineTable<AzureToken>();
            _eagleDatabase.DefineTable<LocalToken>();
            //_eagleDatabase.DefineTable<EventsSignUp>();
            _eagleDatabase.DefineTable<ClassAttendance>();
            _eagleDatabase.DefineTable<Offense>();
            _eagleDatabase.DefineTable<OffenseCategory>();
        }

        /*********************************************************************/
        /*                         Get table references                      */
        /*********************************************************************/
        public void GetReferences()
        {
            _assignmentTable = _client.GetSyncTable<Assignment>();
            _courseTable = _client.GetSyncTable<Course>();
            _fourWindsTable = _client.GetSyncTable<FourWindsItem>();
            _varsityTable = _client.GetSyncTable<VarsityItem>();
            _grabAndGoTable = _client.GetSyncTable<GrabAndGoItem>();
            _studentTable = _client.GetSyncTable<Student>();
            _localTokenTable = _client.GetSyncTable<LocalToken>();
            _azureTokenTable = _client.GetSyncTable<AzureToken>();
            // _eventsSignUpTable = _client.GetSyncTable<EventsSignUp>();
            _attendanceTable = _client.GetSyncTable<ClassAttendance>();
            _offenseTable = _client.GetSyncTable<Offense>();
            _offenseCategoryTable = _client.GetSyncTable<OffenseCategory>();
        }

        /*********************************************************************/
        /*                        Get student assignments                    */
        /*********************************************************************/
        public async Task<List<Assignment>> GetAssignmentsAsync()
        {
            return await _assignmentTable.ToListAsync();
        }

        /*********************************************************************/
        /*                         Get student courses                       */
        /*********************************************************************/
        public async Task<List<Course>> GetCoursesAsync()
        {
            return await _courseTable.ToListAsync();
        }

        /*********************************************************************/
        /*                       Get the Four Winds items                    */
        /*********************************************************************/
        public async Task<List<FourWindsItem>> GetFourWindsItemsAsync()
        {
            return await _fourWindsTable.ToListAsync();
        }

        /*********************************************************************/
        /*                         Get the Varsity items                     */
        /*********************************************************************/
        public async Task<List<VarsityItem>> GetVarsityItemsAsync()
        {
            return await _varsityTable.ToListAsync();
        }

        /*********************************************************************/
        /*                      Get the Grab and Go items                    */
        /*********************************************************************/
        public async Task<List<GrabAndGoItem>> GetGrabAndGoItemsAsync()
        {
            return await _grabAndGoTable.ToListAsync();
        }

        /*********************************************************************/
        /*                       Get the logged in user                      */
        /*********************************************************************/
        /* The database must be purged once the user logs out                */
        public async Task<LocalToken> GetLocalTokenAsync()
        {
            var _localToken = await _localTokenTable.ToListAsync();

            /* Return the current user so that the user does not need to log */
            /* back in if they are still logged in                           */
            if (_localToken.Count != 0)
                return _localToken[0];
            else
                return null;
        }

        /*********************************************************************/
        /*                    Get the Azure login credentials                */
        /*********************************************************************/
        public async Task<AzureToken> GetAzureTokenAsync(LocalToken currentUser)
        {

            //APPARENTLY THIS DOES NOT WORK PROPERLY
            await _azureTokenTable.PullAsync("loginUser",
                _azureTokenTable.Where(user => user.Id == currentUser.Id));

            List<AzureToken> _tokenList = await _azureTokenTable.Where(user =>
                user.Id == currentUser.Id).ToListAsync();

            //DELETE THE LOGIN INFORMATION
            await _azureTokenTable.PurgeAsync();
            return _tokenList[0];
        }

        /*********************************************************************/
        /*                     Get the student information                   */
        /*********************************************************************/
        public async Task<Student> GetStudentAsync()
        {
            List<Student> _students = await _studentTable.ToListAsync();
            return _students[0];
        }

        /*********************************************************************/
        /*                      Insert into local store                      */
        /*********************************************************************/
        public async Task InsertLocalTokenAsync(LocalToken user)
        {
            await _localTokenTable.InsertAsync(user);
        }

        /*********************************************************************/
        /*                      Insert into local store                      */
        /*********************************************************************/
        public async Task PurgeDatabaseAsync()
        {
            await _assignmentTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _courseTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _fourWindsTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _varsityTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _grabAndGoTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _studentTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _localTokenTable.PurgeAsync(null, null, true,
                CancellationToken.None);

            await _azureTokenTable.PurgeAsync(null, null, true,
                CancellationToken.None);
        }

        /*********************************************************************/
        /*                              Get Events                           */
        /*********************************************************************/
        public async Task<List<EventsSignUp>> GetEventsAsync()
        {
            return await _eventsSignUpTable.ToListAsync();
        }

        /*********************************************************************/
        /*                      Get attendance violations                    */
        /*********************************************************************/
        public async Task<List<ClassAttendance>> GetAttendanceViolationsAsync()
        {
            return await _attendanceTable.ToListAsync();
        }

        /*********************************************************************/
        /*                      Get student court offenses                   */
        /*********************************************************************/
        public async Task<List<Offense>> GetStudentCourtOffensesAsync()
        {
            return await _offenseTable.ToListAsync();
        }

        public async Task<List<OffenseCategory>> GetStudentCourtCategoriesAsync()
        {
            return await _offenseCategoryTable.ToListAsync();
        }
    }
}