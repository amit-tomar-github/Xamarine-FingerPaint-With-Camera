using System;

using Android.App;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using Android.Content;
using Android.Provider;
using System.IO;

namespace FingerPaint
{
    [Activity(Label = "Finger Paint", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        FingerPaintCanvasView fingerPaintCanvasView;
        Bitmap bitmap;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the view from the Main.axml layout resource
            SetContentView(Resource.Layout.Main);

            // Get a reference to the FingerPaintCanvasView from the Main.axml file
            fingerPaintCanvasView = FindViewById<FingerPaintCanvasView>(Resource.Id.canvasView);

            // Set up the Spinner to select stroke color
            Spinner colorSpinner = FindViewById<Spinner>(Resource.Id.colorSpinner);
            colorSpinner.ItemSelected += OnColorSpinnerItemSelected;

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.colors_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            colorSpinner.Adapter = adapter;

            // Set up the Spinner to select stroke width
            Spinner widthSpinner = FindViewById<Spinner>(Resource.Id.widthSpinner);
            widthSpinner.ItemSelected += OnWidthSpinnerItemSelected;

            var widthsAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.widths_array, Android.Resource.Layout.SimpleSpinnerItem);
            widthsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            widthSpinner.Adapter = widthsAdapter;

            // Set up the Clear button
            Button clearButton = FindViewById<Button>(Resource.Id.clearButton);
            clearButton.Click += OnClearButtonClick;

            //Open camera and click picture and save background
            Button btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            btnCamera.Click += BtnCamera_Click;

             //Save Image To Local
             Button btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSave_Click; ;
        }

        private void BtnCamera_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var file = System.IO.Path.Combine(path, "signature.png");

                Bitmap imgbitmap = Bitmap.CreateBitmap(fingerPaintCanvasView.Width, fingerPaintCanvasView.Height, Bitmap.Config.Rgb565);

                Canvas canvas = new Canvas(imgbitmap);
                fingerPaintCanvasView.Draw(canvas);

                using (var stream = new FileStream(file, FileMode.Create))
                {
                    imgbitmap.Compress(Bitmap.CompressFormat.Png, 90, stream);
                }

                Toast.MakeText(this,"Image saved",ToastLength.Long).Show();
            }
            catch (Exception ex) { Toast.MakeText(this, ex.Message, ToastLength.Short).Show(); }
        }

        void OnColorSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            Spinner spinner = (Spinner)sender;
            string strColor = (string)spinner.GetItemAtPosition(args.Position);
            Color strokeColor = (Color)(typeof(Color).GetProperty(strColor).GetValue(null));
            fingerPaintCanvasView.StrokeColor = strokeColor;
        }

        void OnWidthSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            Spinner spinner = (Spinner)sender;
            float strokeWidth = new float[] { 2, 5, 10, 20, 50 }[args.Position];
            fingerPaintCanvasView.StrokeWidth = strokeWidth;
        }

        void OnClearButtonClick(object sender, EventArgs args)
        {
            fingerPaintCanvasView.ClearAll();
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            bitmap = (Bitmap)data.Extras.Get("data");
            fingerPaintCanvasView.SetBitMap(bitmap);
        }
    }
}

