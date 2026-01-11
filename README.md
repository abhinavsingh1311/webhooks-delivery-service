# Webhook Delivery Service - Documentation Site

This branch contains the static documentation website for the Webhook Delivery Service project.

## Live Site

**[View Documentation](https://abhinavsingh1311.github.io/webhooks-delivery-service/)**

## Branch Purpose

This `gh-pages` branch hosts only the static HTML/CSS files for GitHub Pages deployment. 

For the actual source code, see the [`main` branch](https://github.com/abhinavsingh1311/webhooks-delivery-service/tree/main).

## Contents

```
docs/
├── index.html      # Main landing page
├── api-docs.html   # API documentation
├── global.css      # Shared styles
├── api.css         # API docs styles
└── favicon.png     # Site icon
```

## Local Preview

To preview locally:

```bash
cd docs
python -m http.server 8000
# or
npx serve .
```

Then open `http://localhost:8000`

## Related Links

- [Main Repository](https://github.com/abhinavsingh1311/webhooks-delivery-service)
- [Source Code](https://github.com/abhinavsingh1311/webhooks-delivery-service/tree/main)
- [Issues](https://github.com/abhinavsingh1311/webhooks-delivery-service/issues)

---

*This branch is automatically deployed via GitHub Pages.*
