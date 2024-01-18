package com.alinagari.macromate

import android.content.Intent
import android.os.Build
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.ImageButton
import android.widget.RelativeLayout
import androidx.annotation.RequiresApi
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

        val macroBtns: Array<RelativeLayout> = Array(10) { RelativeLayout(this) }
        macroBtns[0] = binding.btn00
        macroBtns[1] = binding.btn01
        macroBtns[2] = binding.btn02
        macroBtns[3] = binding.btn03
        macroBtns[4] = binding.btn04
        macroBtns[5] = binding.btn10
        macroBtns[6] = binding.btn11
        macroBtns[7] = binding.btn12
        macroBtns[8] = binding.btn13
        macroBtns[9] = binding.btn14

        macroBtns[0].setOnClickListener { socketMessage("00") }
        macroBtns[1].setOnClickListener { socketMessage("01") }
        macroBtns[2].setOnClickListener { socketMessage("02") }
        macroBtns[3].setOnClickListener { socketMessage("03") }
        macroBtns[4].setOnClickListener { socketMessage("04") }
        macroBtns[5].setOnClickListener { socketMessage("10") }
        macroBtns[6].setOnClickListener { socketMessage("11") }
        macroBtns[7].setOnClickListener { socketMessage("12") }
        macroBtns[8].setOnClickListener { socketMessage("13") }
        macroBtns[9].setOnClickListener { socketMessage("14") }
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
}