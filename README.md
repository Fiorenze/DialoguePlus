# DialoguePlus
A simple, customizable dialogue engine for Unity — inspired by Ren'Py-style markup, but written entirely from scratch in C#.  
- It’s designed for narrative-driven interactive games where you want flexible conditions, variables, branching choices, and inline actions.
- See DemoDialogue.dlg for more information about current capabilties (WIP)

---

## **Key Features**

✅ Custom markup language (`.dlg` files) with a familiar structure:
- `label` and `jump` for scene flow
- `if`, `elif`, `else` for conditional branches
- `menu:` for player choices
- Inline variable references: `Hello, {playerName}!`
- Actions: `$myVar += 1`  
- Supports `int`, `bool`, and `string` definitions

✅ Written 100% in C# for Unity — no external dependencies.

---

## **Current Status**

This is a **work in progress**!  
✔️ Core parser works: labels, dialogue lines, variables, conditions, menus  
✔️ Supports runtime variable injection and actions
✔️ Supports rollback

❌ **Not implemented yet:**
- Changing scenes or backgrounds (`scene` command)
- Showing/hiding character images (`show` / `hide`)

---

## **Example Syntax**

```plaintext
label start

	System "This is a text based interactive dialogue system."

	System "You can define; bool, integer and names."
	
	System "You can also interact with them in dialogue."
	
	System "For example, {score} will show the value of 'score'."

	menu:
		"This option will increase score by 50"
			$score += 50
		"Set score to 100"
			$score = 100
		"This option only shows if score >= 500" if score >= 500
			"You will see this only if you have chosen third option"
		
	jump nextScene

label nextScene

	"You jumped to the next scene!"
```
