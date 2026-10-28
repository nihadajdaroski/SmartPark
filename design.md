---
version: "1.0"
name: "SmartPark Analytics Dashboard"
description: "Clean, functional dashboard system for SmartPark with light neutral surfaces, indigo actions, green success states, readable data tables and restrained card elevation."
colors:
  primary: "#4f46e5"
  background: "#f5f6f8"
  surface: "#ffffff"
  brand-blue: "#1d77ff"
  brand-blue-soft: "#eaf3ff"
  text: "#1b2130"
  accent: "#16a34a"
  on-primary: "#FFFFFF"
  on-surface: "#1b2130"
  on-accent: "#000000"
typography:
  h1:
    fontFamily: "'Inter', system-ui, sans-serif"
    fontSize: "4rem"
    fontWeight: "700"
    lineHeight: "1.05"
  body-md:
    fontFamily: "'Inter', 'Helvetica Neue', system-ui, -apple-system, sans-serif"
    fontSize: "1rem"
    fontWeight: "400"
    lineHeight: "1.6"
  label-caps:
    fontFamily: "'Inter', 'Helvetica Neue', system-ui, -apple-system, sans-serif"
    fontSize: "0.75rem"
    fontWeight: "600"
    letterSpacing: "0.06em"
rounded:
  sm: "6px"
  md: "10px"
  lg: "14px"
spacing:
  sm: "8px"
  md: "16px"
  lg: "32px"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.on-primary}"
    rounded: "{rounded.md}"
    padding: "16px"
  button-secondary:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.on-accent}"
    rounded: "{rounded.md}"
    padding: "16px"
  card:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.on-surface}"
    rounded: "{rounded.lg}"
    padding: "24px"
  page:
    backgroundColor: "{colors.background}"
    textColor: "{colors.text}"
    padding: "16px"
---
## Overview

SmartPark dashboard visual system based on the Analytics Dashboard structure. It keeps the interface clean and functional while using indigo for primary actions and navigation, green for successful states, and neutral surfaces for data-heavy views.

- **Category:** flat-platform
- **Era:** Ongoing
- **Origin:** SmartPark product UI.
- **Reference:** Stripe Dashboard; Vercel Analytics; admin panels.
- **Structural base:** application shell with navigation, maps, forms, cards and tables.

**Defining traits:**
- Card grids, KPI tiles, maps, tables and forms
- Neutral surfaces with indigo primary actions and green success states
- Dense but scannable, clear hierarchy
- Function-first legibility

## Colors

- **bg** — #f5f6f8
- **surface** — #ffffff
- **surface-strong** — #eef0f4
- **border** — #e2e5eb
- **text** — #1b2130
- **text-muted** — #5b6274
- **primary** — #4f46e5
- **primary-active** — #4338ca
- **brand-blue** — #1d77ff, used for the SmartPark logo and navigation hover state
- **brand-blue-soft** — #eaf3ff, used as the navigation hover and active background
- **accent** — #16a34a
- **negative** — #dc2626

## Typography

- Display: 'Inter', system-ui, sans-serif
- Body: 'Inter', 'Helvetica Neue', system-ui, -apple-system, sans-serif
- Mono: 'IBM Plex Mono', ui-monospace, monospace

## Layout

- shell de aplicação com navegação e cartões
- Use a responsive grid and preserve content hierarchy.
- Collapse columns below 768px without horizontal overflow.

## Elevation & Depth

- Use subtle borders before shadows. Cards should be scannable and functional, with enough spacing for map controls and tables.

## Shapes

- Radius scale: 6px, 10px, 14px.

## Components

- Buttons keep visible focus and predictable hover/pressed states.
- Cards use the surface and radius scale from the tokens.
- Forms expose persistent labels, textual errors, and keyboard focus.
- Password and profile forms use a compact header, clear labels and a visible secondary return action.

## Do's and Don'ts

- Do: follow the tokens and the structural composition.
- Do: use indigo for primary actions, active navigation and focus states.
- Do: use green for successful availability and payment states.
- Do: keep the map as the main visual focus on the home page.
- Do: maintain WCAG AA contrast and `prefers-reduced-motion`.
- Avoid: using the style as decoration without functional hierarchy.

<!-- Source: https://designmd.app/library/dashboard-analytics · designmd.app -->
