package com.alinagari.macromate

import android.content.Intent
import android.graphics.BitmapFactory
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import androidx.appcompat.app.AppCompatActivity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.io.BufferedInputStream
import java.io.IOException
import java.net.Socket
import java.nio.ByteBuffer

class DataActivity : AppCompatActivity() {
    private var IP: String? = null
    private var PORT: Int = 0
    private var llout: LinearLayout? = null
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_data)
        IP = intent.getStringExtra("IP")
        PORT = intent.getIntExtra("PORT", 0)
        socketMessage("connected")

        llout = findViewById(R.id.llout)

        var btn = findViewById<Button>(R.id.btnGet)
        btn.setOnClickListener {
            receiveImages()
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

    private fun receiveImages() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val socket = Socket(IP, PORT)
                val inputStream = BufferedInputStream(socket.getInputStream())

                val lenByt = ByteArray(4)
                var bRd = inputStream.read(lenByt)
                val numRows = ByteBuffer.wrap(lenByt.copyOf(bRd)).int
                Log.d("DDD", numRows.toString())

                bRd = inputStream.read(lenByt)
                val numCols = ByteBuffer.wrap(lenByt.copyOf(bRd)).int
                Log.d("DDD", numCols.toString())

                for (i in 1..numRows * numCols + 2) {
                    Thread.sleep(200)
                    bRd = inputStream.read(lenByt)
                    val imgSize = ByteBuffer.wrap(lenByt.copyOf(bRd)).int
                    Log.d("IMG", imgSize.toString())

                    val imgData = ByteArray(imgSize)
                    inputStream.read(imgData, 0, imgSize)

                    try {
                        val bmp = BitmapFactory.decodeByteArray(imgData, 0, imgSize)
                        //bitmaps.add(bmp)
                        val app = application as AppBitmap
                        app.bitmaps.add(bmp)
                        runOnUiThread {
                            val imageView = ImageView(applicationContext)
                            imageView.setImageBitmap(bmp)
                            imageView.minimumWidth = 100
                            imageView.minimumHeight = 100
                            imageView.maxWidth = 100
                            imageView.maxHeight = 100
                            llout?.addView(imageView)
                        }
                    } catch (ex:Exception) {
                        Log.e("EX", ex.localizedMessage)
                    }
                }
                socket.close()
                val intent = Intent(applicationContext, MacroActivity::class.java)
                intent.putExtra("IP", IP)
                intent.putExtra("PORT", PORT)
                intent.putExtra("ROWS", numRows)
                intent.putExtra("COLS", numCols)
                startActivity(intent)

            } catch (ex: Exception) {
                Log.e("Error", ex.message ?: "Unknown error")
            }
        }
    }

}