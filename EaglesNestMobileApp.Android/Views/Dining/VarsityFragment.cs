/*****************************************************************************/
/*                              varsityFragment                              */
/*                                                                           */
/*****************************************************************************/
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using EaglesNestMobileApp.Core.ViewModel.DiningViewModels;
using EaglesNestMobileApp.Core;
using GalaSoft.MvvmLight.Helpers;
using EaglesNestMobileApp.Core.Model;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Widget;
using static Android.Support.Design.Widget.TabLayout;
using System.Collections.Generic;
using EaglesNestMobileApp.Core.ViewModel;

namespace EaglesNestMobileApp.Android.Views.Dining
{
    public class varsityFragment : Fragment
    {
        private View _varsityFragmentView;
        List<RecyclerView> RecyclerviewList;
        List<TextView> LineList;
        RecyclerView _currentRecyclerview;
        RecyclerView _previousRecyclerview;
        int _lineCount;

        public VarsityFragmentViewModel ViewModel
        {
            get { return App.Locator.Varsity; }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _lineCount = ViewModel.VarsityMenu.LunchMenu.Count;
        }

        public override View OnCreateView(LayoutInflater inflater,
            ViewGroup container, Bundle savedInstanceState)
        {
            /* Use this to return your custom view for this Fragment         */
            _varsityFragmentView =
                inflater.Inflate(Resource.Layout.VarsityFragmentLayout,
                    container, false);

            /* Set up the tablayout for the meal times                       */
            TabLayout _tabLayout =
                _varsityFragmentView.FindViewById<TabLayout>(Resource.Id.varsityTabs);

            /* Set the current and previous recyclerviews */
            _currentRecyclerview = _previousRecyclerview =
                _varsityFragmentView.FindViewById<RecyclerView>(Resource.Id.VarsityLine1);

            /* Set up the recyclerviews and adapters for the fragment        */
            Activity.RunOnUiThread(() => SetUpVarsity());

            return _varsityFragmentView;
        }


        /* Get the recyclerviews in from the xml layout                      */
        private void SetUpVarsity()
        {
            /* Add the recyclerviews to a list                               */
            RecyclerviewList = new List<RecyclerView>
            {
                _varsityFragmentView.FindViewById<RecyclerView>(Resource.Id.VarsityLine1),
                _varsityFragmentView.FindViewById<RecyclerView>(Resource.Id.VarsityLine2),
                _varsityFragmentView.FindViewById<RecyclerView>(Resource.Id.VarsityLine3),
                _varsityFragmentView.FindViewById<RecyclerView>(Resource.Id.VarsityLine4),
                _varsityFragmentView.FindViewById<RecyclerView>(Resource.Id.VarsityLine5)
            };

            /* Add the lines to a list                                       */
            LineList = new List<TextView>
            {
                _varsityFragmentView.FindViewById<TextView>(Resource.Id.varsityOption1),
                _varsityFragmentView.FindViewById<TextView>(Resource.Id.varsityOption2),
                _varsityFragmentView.FindViewById<TextView>(Resource.Id.varsityOption3),
                _varsityFragmentView.FindViewById<TextView>(Resource.Id.varsityOption4),
                _varsityFragmentView.FindViewById<TextView>(Resource.Id.varsityOption5)
            };

            /* Set adapters, layout managers, and click handlers             */
            for (int count = 0; count < _lineCount; count++)
            {
                /* Set Adapters */
                RecyclerviewList[count].SetAdapter
                    (
                        ViewModel.VarsityMenu.LunchMenu[count].GetRecyclerAdapter(BindViewHolder, Resource.Layout.FoodMenuList)
                    );

                /* Set Layout Managers */
                RecyclerviewList[count].SetLayoutManager(new LinearLayoutManager(Activity));

                /* Set Click Event */
                LineList[count].Click += LineClick;
            }
        }

        private void LineClick(object sender, System.EventArgs e)
        {
            /* Find the correct recyclerview, close the previous one         */
            for (int count = 0; count < LineList.Count; count++)
            {
                if (sender.Equals(LineList[count]))
                {
                    _currentRecyclerview = RecyclerviewList[count];

                    if (_previousRecyclerview == _currentRecyclerview &&
                        _previousRecyclerview.Visibility == ViewStates.Visible)
                        _currentRecyclerview.Visibility = ViewStates.Gone;
                    else
                    {
                        if (_previousRecyclerview != _currentRecyclerview)
                            _previousRecyclerview.Visibility = ViewStates.Gone;
                        _currentRecyclerview.Visibility = ViewStates.Visible;
                    }
                }
                else
                    System.Diagnostics.Debug.Write("Something is very wrong: in the varsity fragment");
            }
            _previousRecyclerview = _currentRecyclerview;
        }

        private void BindViewHolder(CachingViewHolder holder, VarsityItem varsityItem, int position)
        {
            TextView _textview = holder.FindCachedViewById<TextView>(Resource.Id.listItem);

            holder.DeleteBinding(_textview);

            var itemBinding = new Binding<string, string>(
                varsityItem,
                () => varsityItem.ItemName,
                _textview,
                () => _textview.Text,
                BindingMode.OneWay,
                "Closed"
               );

            holder.SaveBinding(_textview, itemBinding);
        }
    }
}