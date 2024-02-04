package com.alinagari.macromate

import android.content.Intent
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.ImageView
import com.alinagari.macromate.databinding.ActivityMainBinding
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers.IO
import kotlinx.coroutines.launch
import java.io.BufferedReader
import java.io.BufferedWriter
import java.io.IOException
import java.io.InputStreamReader
import java.io.OutputStreamWriter
import java.net.Socket
import java.util.Scanner

class MainActivity : AppCompatActivity() {

    private var img: ImageView? = null
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        val binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        img = binding.img

        val btnConnect = binding.btnConnect
        val ipText = binding.etIpAddress
        val portText = binding.etPortNumber

        btnConnect.setOnClickListener {
            CoroutineScope(IO).launch {
                socketConnect(ipText.text.toString(), portText.text.toString().toInt())
            }
        }
    }

    private fun socketConnect(sip: String, sport: Int) {
        Log.d("SocketConnect", "sip $sip $sport")
        try {
            val socket = Socket(sip, sport)
            val dataOutputStream = socket.getOutputStream()
            dataOutputStream.write("request".toByteArray())
            val reader = BufferedReader(InputStreamReader(socket.getInputStream()))
            val response = reader.readLine()
            Log.d("RESPONSE", response)
            dataOutputStream.close()
            socket.close()
            if (response == "connect") {
                val intent = Intent(this, DataActivity::class.java)
                intent.putExtra("IP", sip)
                intent.putExtra("PORT", sport)
                startActivity(intent)
            }
        } catch (e: IOException) {
            e.printStackTrace()
            Log.w("Exception", e.localizedMessage)
        }
    }
}

