package com.alinagari.macromate

import android.content.Intent
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Base64
import android.util.Log
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.Toast
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.io.BufferedInputStream
import java.io.BufferedReader
import java.io.ByteArrayOutputStream
import java.io.DataInputStream
import java.io.IOException
import java.io.InputStreamReader
import java.net.Socket
import java.nio.ByteBuffer
import java.nio.ByteOrder

class DataActivity : AppCompatActivity() {
    private var IP: String? = null
    private var PORT: Int = 0
    private var llout: LinearLayout? = null
    val bitmaps = mutableListOf<Bitmap?>()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_data)
        IP = intent.getStringExtra("IP")
        PORT = intent.getIntExtra("PORT", 0)
        socketMessage("connected")

        llout = findViewById(R.id.llout)

        var btn = findViewById<Button>(R.id.btnGet)
        btn.setOnClickListener {
            Toast.makeText(this, bitmaps.size.toString(), Toast.LENGTH_SHORT).show();
            /*for (img in bitmaps) {
                val imageView = ImageView(this)
                imageView.setImageBitmap(img)
                imageView.minimumWidth = 100
                imageView.minimumHeight = 100
                imageView.maxWidth = 100
                imageView.maxHeight = 100
                llout?.addView(imageView)
            }*/
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

    private fun BitmapToString(bitmap: Bitmap): String {
        val byteArrayOutputStream = ByteArrayOutputStream()
        bitmap.compress(Bitmap.CompressFormat.PNG, 100, byteArrayOutputStream)
        val byteArray = byteArrayOutputStream.toByteArray()
        return Base64.encodeToString(byteArray, Base64.DEFAULT)
    }

    /*
        private fun receiveImages() {
            CoroutineScope(Dispatchers.IO).launch {
                val endMarker = "<---[EM]--->"
                try {
                    val socket = Socket(IP, PORT)
                    val inputStream = socket.getInputStream()

                    val lengthBytes = ByteArray(4)
                    inputStream.read(lengthBytes)
                    val numRows = ByteBuffer.wrap(lengthBytes).int
                    Log.d("DDD", numRows.toString())
                    inputStream.read(lengthBytes)
                    val numCols = ByteBuffer.wrap(lengthBytes).int
                    Log.d("DDD", numCols.toString())

                    for (i in 1..numRows * numCols + 2) {

                        val byr = inputStream.read(lengthBytes)
                        val imgSize = ByteBuffer.wrap(lengthBytes).int
                        Log.d("IMG", imgSize.toString())

                        val imgData = ByteArray(imgSize)
                        inputStream.read(imgData)

                        val bmp = BitmapFactory.decodeByteArray(imgData, 0, imgSize)
                        runOnUiThread {
                            val imageView = ImageView(applicationContext)
                            imageView.setImageBitmap(bmp)
                            imageView.minimumWidth = 100
                            imageView.minimumHeight = 100
                            imageView.maxWidth = 100
                            imageView.maxHeight = 100
                            llout?.addView(imageView)
                        }
                       // Thread.sleep(150)
                    }




                    socket.close()
                } catch (ex: Exception) {

                }
            }
        }
    */
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

                    val bmp = BitmapFactory.decodeByteArray(imgData, 0, imgSize)
                    runOnUiThread {
                        val imageView = ImageView(applicationContext)
                        imageView.setImageBitmap(bmp)
                        imageView.minimumWidth = 100
                        imageView.minimumHeight = 100
                        imageView.maxWidth = 100
                        imageView.maxHeight = 100
                        llout?.addView(imageView)
                    }
                }
                socket.close()
            } catch (ex: Exception) {
                Log.e("Error", ex.message ?: "Unknown error")
            }
        }
    }

}