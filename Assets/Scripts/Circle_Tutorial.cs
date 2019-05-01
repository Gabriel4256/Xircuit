﻿using UnityEngine;
using System.Collections;

public class Circle_Tutorial : MonoBehaviour {

	Vector2 pos_init;
	Color color_init;
	// (tag) Black: 0, Red: 1, White: 7
	int tag_init;

	int tag;
	// Circle Overlapped By 'this'
	GameObject target;
	bool is_clicked = false;

	GameObject wave;
	Color color_wave;
	bool is_waving = false;
	IEnumerator coroutine_wave;
	public GameObject[] circles_other;

	public Color GetColor () {
		return new Color (color_init.r, color_init.g, color_init.b, 1.0f);
	}

	public int GetTag () {
		return this.tag;
	}

	void Start () {
		// Initiate Variables
		this.pos_init = transform.position;
		this.color_init = GetComponent<SpriteRenderer>().color;

		this.tag = int.Parse (gameObject.tag);

		this.wave = transform.GetChild (0).gameObject;
		color_wave = wave.GetComponent<SpriteRenderer> ().color;
	}

	void OnMouseDrag () {
		this.is_clicked = true;

		GetComponent<SpriteRenderer> ().color = new Color (color_init.r, color_init.g, color_init.b, 1.0f);

		if (!is_waving) {
			coroutine_wave = MakeWave ();
			StartCoroutine (coroutine_wave);
		}

		// Move Position
		Vector2 pos = Input.mousePosition;
		Vector2 pos_real = Camera.main.ScreenToWorldPoint (pos);
		transform.position = new Vector3 (pos_real.x, this.pos_init.y, -3);
	}

	void OnMouseUp () {
		this.is_clicked = false;

		GetComponent<SpriteRenderer> ().color = color_init;

		if (is_waving) {
			StopCoroutine (coroutine_wave);
			is_waving = false;
			wave.SetActive (false);
			wave.GetComponent<SpriteRenderer> ().color = color_wave;
		}

		// Initialize Position
		transform.position = new Vector3 (this.pos_init.x, this.pos_init.y, 0);

		if (this.target != null) {
			GameObject.Find ("Instruction").GetComponent<Instruction> ().GetColor (this.tag, target.GetComponent<Circle_Tutorial> ().GetTag ());
			this.target.GetComponent<Circle_Tutorial> ().ChangeColor (this.tag);
			for (int i = 0; i < this.circles_other.Length; i++) {
				if (this.circles_other [i] == target)
					continue;
				this.circles_other [i].GetComponent<Circle_Tutorial> ().Remove ();
			}
			this.target = null;
			gameObject.SetActive (false);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (this.is_clicked) {
			this.target = other.gameObject;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (this.target == other.gameObject) {
			this.target = null;
		}
	}

	public void ChangeColor (int tag) {
		this.tag = this.tag ^ tag;
		GetComponent<SpriteRenderer> ().color = GameObject.FindWithTag (this.tag.ToString ()).GetComponent<Circle_Tutorial> ().GetColor ();
		GetComponent <CircleCollider2D> ().enabled = false;
		StartCoroutine (Move ());
	}

	public void Remove () {
		StartCoroutine (FadeOut ());
	}

	IEnumerator FadeOut () {
		Color color = GetComponent<SpriteRenderer> ().color;
		float a = color.a;
		while (a >= 0.0f) {
			a -= 0.05f;
			GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, a);
			yield return new WaitForSeconds (0.025f);
		}
		gameObject.SetActive (false);
	}

	IEnumerator MakeWave () {
		is_waving = true;

		wave.SetActive (true);

		while (true) {
			float period;
			for (period = 0.0f; period < 20.0f; period++) {
				wave.transform.localScale = 1.3f * new Vector3 (1.0f + 0.01f * period, 1.0f + 0.01f * period, 0.0f);
				wave.GetComponent<SpriteRenderer> ().color -= new Color (0.0f, 0.0f, 0.0f, 0.03725f);
				yield return new WaitForSeconds(0.075f);
			}
			wave.transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);
			wave.GetComponent<SpriteRenderer> ().color += new Color(0.0f, 0.0f, 0.0f, 0.745f);
			yield return new WaitForSeconds (1.5f);
		}
	}

	IEnumerator Move () {
		yield return new WaitForSeconds (0.25f);

		float t = 0.0f;
		while (t <= 1.0f) {
			t += 0.05f;
			transform.position = Vector3.Lerp (pos_init, new Vector3(0.0f,0.0f,0.0f), t);
			yield return new WaitForSeconds (0.025f);
		}
	}
}
