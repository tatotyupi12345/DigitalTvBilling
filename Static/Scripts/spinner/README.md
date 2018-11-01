# gSpinner

---

## A jQuery Plugin for Customizable Loaders

![g-spinner](images/g-spinner.gif)

---

### Installation

---

#### Using a Script Tag

Include script after the jQuery library:

```html
<script src="/path/to/g-spinner.min.js"></script>
```

#### Using a CommonJs

Require g-spinner **after** requiring jQuery

```javascript
const $ = require('jquery')
require('../src/js/g-spinner');
```

#### Usage

* Select an element with which to append the spinner to, run the gSpinner method.

```javascript
// Getting a reference to the container we will append the spinner to. This can be any element of any id or class.
var $loader = $("#loader");
// Starting the spinner
$loader.gSpinner();
```

* To hide the spinner, simply pass "hide" into the gSpinner method.

```javascript
$loader.gSpinner("hide");
```

* Alternatively, you may pass an options object into the gSpinner method:

```javascript
$loader.gSpinner({ loading: true, scale: .5 });
```

| Property    |               Explanation                |
| :---------: | :--------------------------------------- |
| `loading`   | Can be `true` or `false` . Set to true if no value is given. Loads and hides the spinner. |
|  `scale`    | The relative size of the spinner, with `1` being full sized at 200x200px, and .5 being 1/2 sized at 100x100px. Set to 1 by default if no value is given. |

### What's Next

---

* Different types of loaders

* Ability to customize a loader's colors

### How to Contribute

---

* Create an Issue

  * Be sure to include the following for bugs:

    * Your OS & version

    * The issue

    * Steps to reproduce the issue

* Or feel free to open a pull request

### Authors

---

[Christian Eckenrode](https://github.com/ceckenrode)
