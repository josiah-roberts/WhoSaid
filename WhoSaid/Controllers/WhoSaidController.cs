using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WhoSaid.Controllers
{
    [ApiController]
    [Route("")]
    public class WhoSaidController : ControllerBase
    {
        const string WhoSaidHtml = @"<html>
  <head>
    <title>Who said...?</title>
    <style>
      body {
        font-family: sans-serif;
        max-width: 800px;
        margin-left: auto;
        margin-right: auto;
        background: #333;
        color: #eee;
      }

      #said {
        width: 100%;
        font-size: 2em;
        border-radius: 0.1em;
        border: none;
        background: white;
        color: #333;
        padding: 0.5em;
        cursor: text;
      }

      [contenteditable='true']:empty:before {
        content: attr(placeholder);
        display: block; /* For Firefox */
        color: grey;
      }

      #lebars > div {
        position: relative;
        font-size: 2em;
        padding-top: 1em;
        padding-bottom: 1em;
        padding-left: 0.5em;
        margin-top: 0.5em;
        margin-bottom: 0.5em;
      }

      #lebars div:not(.bar) {
        z-index: 2;
        position: relative;
      }

      #lebars .bar {
        transition: width 0.5s;
        position: absolute;
        height: 100%;
        width: 20%;
        top: 0;
        left: 0;
        z-index: 1;
      }
    </style>
    <script type='text/javascript' defer>
      const debounce = (func, ms) => {
        let timeout;
        return (...args) => {
          clearTimeout(timeout);
          timeout = setTimeout(() => {
            timeout = null;
            func(...args);
          }, ms);
        };
      };

      const pick = (collection) =>
        collection[Math.floor(Math.random() * collection.length)];

      const reLayout = (prediction) => {
        const lebars = document.querySelector('#lebars');
        for (const { label, confidence } of prediction.scores) {
          const bar = document.querySelector(`#${label}`);
          lebars.append(bar);
          setTimeout(() => {
            const innerBar = document.querySelector(`#${label} .bar`);
            innerBar.style.width = `${Math.round(confidence * 100, 2).toFixed(
              2
            )}%`;
          }, 100);
        }
      };

      const reRender = (prediction) => {
        const beforeforeTimes = prediction.scores
          .map(({ label }) => document.querySelector(`#${label}`))
          .map((elm) => ({ elm, first: elm.getBoundingClientRect() }));

        reLayout(prediction);

        const afterward = beforeforeTimes.map((b) => ({
          ...b,
          last: b.elm.getBoundingClientRect(),
        }));

        for (const { elm, first, last } of afterward) {
          // Invert: determine the delta between the
          // first and last bounds to invert the element
          const deltaX = first.left - last.left;
          const deltaY = first.top - last.top;
          const deltaW = first.width / last.width;
          const deltaH = first.height / last.height;

          // Play: animate the final element from its first bounds
          // to its last bounds (which is no transform)
          elm.animate(
            [
              {
                transformOrigin: 'top left',
                transform: `
                    translate(${deltaX}px, ${deltaY}px)
                    scale(${deltaW}, ${deltaH})
                `,
              },
              {
                transformOrigin: 'top left',
                transform: 'none',
              },
            ],
            {
              duration: 500,
              easing: 'ease-in-out',
              fill: 'both',
            }
          );
        }
      };

      const predictMeBaby = debounce(async (event) => {
        const de = document.documentElement;
        const ta = event.target;
        window.location.hash = encodeURIComponent(ta.innerText);
        const prediction = await fetch(
          `/whosaid?text=${encodeURIComponent(
            ta.innerText
          )}`
        ).then((x) => x.json());
        reRender(prediction);
      }, 300);

      window.onload = () => {
        const prompts = [
          `Yeah, tetanus is not to be fucked with`,
          `Fuck the world, etc.`,
          `Yeah that's pretty fuckin classy.`,
          `Holy FUCK`,
          `Welcome to the rice fields, motherfucker`,
          `Blast the fuckers out of the air.`,
          `This whole subreddit is dank as fuck.`,
          `I am at the grocery store. It's fucking insane`,
          `Fuck this day in general.`,
          `I'm very fucking stressed right now.`,
          `Fuck you and your gorgeous bone structure`,
          `fuck fuckity fuck the fucking fucked fuck fucker`,
          `And of fucking course you're invited.`,
          `Fuck a duck. It happened again.`,
          `IMPORTANT FUCKING UPDATE`,
          `Absolutely fucking beautiful.`,
          `Top fucking kek.`,
          `...... Fuck did I just walk in on?`,
          `It's fuck it o'clock at The Duce y'all`,
          `I DONATED, YOU FUCKS.`,
          `Like fuck am I that old already?`,
          `The dude to the right is a total fucking nerd.`,
          `That's a specific fucking meme`,
          `I am approximately fucking psyched.`,
          `Oops. I seem to have misplaced my fucks.`,
          `what the ever living fuck`,
          `I have fucked up.`,
          `I have so many fucking snacks.`,
          `This is so fucking wholesome...`,
          `ooohhh fuck these unethical shenanigans V. MUCH`,
        ];
        const said = document.querySelector('#said');
        said.addEventListener('input', predictMeBaby);
        said.setAttribute('placeholder', pick(prompts));
        if (window.location.hash && window.location.hash !== '#') {
          said.innerText = decodeURIComponent(window.location.hash.substr(1));
          predictMeBaby({target: said});
        }
      };
    </script>
  </head>
  <body>
    <h1>Who said...?</h1>
    <h2>Inquiring minds want to know.</h2>
    <div>
      <div class='text' id='said' contenteditable='true'></div>
    </div>
    <div id='lebars'>
      <div id='josiah'>
        <div>Josiah</div>
        <div class='bar' style='background-color: #5555aaaa;'></div>
      </div>
      <div id='jackie'>
        <div>Jackie</div>
        <div class='bar' style='background-color: #aa55aaaa;'></div>
      </div>
      <div id='danielle'>
        <div>Danielle</div>
        <div class='bar' style='background-color: #aaaa55cc;'></div>
      </div>
      <div id='adam'>
        <div>Adam</div>
        <div class='bar' style='background-color: #aa5555aa;'></div>
      </div>
      <div id='steven'>
        <div>Steven</div>
        <div class='bar' style='background-color: #55aa55aa;'></div>
      </div>
    </div>
  </body>
</html>
";

        private readonly ILogger<WhoSaidController> _logger;

        public WhoSaidController(ILogger<WhoSaidController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get() =>
            Content(WhoSaidHtml, "text/html");

        [HttpGet("whosaid")]
        public ActionResult WhoSaid(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return StatusCode(400);
            }
            return this.StatusCode(200, ConsumeModel.Predict(text.ToLower()));
        }
    }
}
