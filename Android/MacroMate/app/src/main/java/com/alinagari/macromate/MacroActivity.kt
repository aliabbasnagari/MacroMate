package com.alinagari.macromate

import android.content.Context
import android.content.Intent
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.os.Build
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.GridLayout
import android.widget.ImageButton
import android.widget.RelativeLayout
import androidx.annotation.RequiresApi
import androidx.core.content.ContextCompat
import androidx.core.view.marginBottom
import androidx.core.view.setPadding
import com.alinagari.macromate.databinding.ActivityMacroBinding
import com.alinagari.macromate.databinding.ActivityMainBinding
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.io.IOException
import java.net.Socket
import java.util.Scanner

class MacroActivity : AppCompatActivity() {

    private var IP: String? = null
    private var PORT: Int = 0
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_macro)
        hideSystemUI()

        val binding = ActivityMacroBinding.inflate(layoutInflater)
        setContentView(binding.root)

        IP = intent.getStringExtra("IP")
        PORT = intent.getIntExtra("PORT", 0)
        socketMessage("connected")


        val macroBtns = Array(4) { RelativeLayout(this) }
        macroBtns[0] = binding.btn0
        macroBtns[1] = binding.btn1
        macroBtns[2] = binding.btn2
        macroBtns[3] = binding.btn3

        macroBtns[0].setOnClickListener { socketMessage("1") }
        macroBtns[1].setOnClickListener { socketMessage("2") }
        macroBtns[2].setOnClickListener { socketMessage("3") }
        macroBtns[3].setOnClickListener { socketMessage("4") }


        val gridBtn = binding.gridBtn
        gridBtn.rowCount = 2
        gridBtn.columnCount = 5

        var msgIdx = 5
        for (i in 0 until gridBtn.rowCount) {
            for (j in 0 until gridBtn.columnCount) {
                val button = ImageButton(this)
                button.setBackgroundResource(R.drawable.btn_macro)
                button.setImageBitmap(getResizedBitmap(this, R.drawable.ic_arrow, 30, 30))
                button.layoutParams = GridLayout.LayoutParams().apply {
                    width = 0
                    height = 0
                    columnSpec = GridLayout.spec(GridLayout.UNDEFINED, 1f)
                    rowSpec = GridLayout.spec(GridLayout.UNDEFINED, 1f)
                    setGravity(Gravity.FILL)
                    setMargins(14, 14, 14, 14)
                }
                val msg = msgIdx.toString()
                button.setOnClickListener { socketMessage(msg) }
                gridBtn.addView(button)
                msgIdx++
            }
        }
    }

    private fun socketMessage(message: String) {
        Log.d("socketMessage", "sip $IP $PORT")
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val socket = Socket(IP, PORT)
                val dataOutputStream = socket.getOutputStream()
                dataOutputStream.write(message.toByteArray())
                dataOutputStream.close()
                socket.close()
            } catch (e: IOException) {
                e.printStackTrace()
                e.localizedMessage?.let { Log.w("Exception", it) }
            }
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        socketMessage("exit")
    }

    @Deprecated("Deprecated in Java")
    override fun onBackPressed() {
        super.onBackPressed()
        finish()
    }

    private fun hideSystemUI() {
        window.decorView.systemUiVisibility =
            (View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                    or View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                    or View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                    or View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                    or View.SYSTEM_UI_FLAG_FULLSCREEN
                    or View.SYSTEM_UI_FLAG_HIDE_NAVIGATION)
    }

    private fun getResizedBitmap(
        context: Context,
        resourceId: Int,
        targetWidth: Int,
        targetHeight: Int
    ): Bitmap {
        val options = BitmapFactory.Options()
        options.inJustDecodeBounds = true
        BitmapFactory.decodeResource(context.resources, resourceId, options)
        options.inSampleSize = calculateInSampleSize(options, targetWidth, targetHeight)
        options.inJustDecodeBounds = false
        return BitmapFactory.decodeResource(context.resources, resourceId, options)
    }

    private fun calculateInSampleSize(
        options: BitmapFactory.Options,
        reqWidth: Int,
        reqHeight: Int
    ): Int {
        val (rawWidth, rawHeight) = options.run { outWidth to outHeight }

        var inSampleSize = 1
        if (rawWidth > reqWidth || rawHeight > reqHeight) {
            val halfWidth = rawWidth / 2
            val halfHeight = rawHeight / 2
            while ((halfWidth / inSampleSize) >= reqWidth && (halfHeight / inSampleSize) >= reqHeight) {
                inSampleSize *= 2
            }
        }
        return inSampleSize
    }
}