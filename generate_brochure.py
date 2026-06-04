from reportlab.lib.pagesizes import A4
from reportlab.lib import colors
from reportlab.lib.units import mm, cm
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_JUSTIFY
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
    HRFlowable, PageBreak, KeepTogether
)
from reportlab.graphics.shapes import Drawing, Rect, String, Circle, Line
from reportlab.graphics import renderPDF
from reportlab.platypus.flowables import Flowable
import os

# ── Colour palette ────────────────────────────────────────────────────────────
NAVY      = colors.HexColor("#0D1B2A")
TEAL      = colors.HexColor("#1B7F8E")
GOLD      = colors.HexColor("#F5A623")
LIGHT_BG  = colors.HexColor("#F4F7FA")
MID_GREY  = colors.HexColor("#8A9BAE")
WHITE     = colors.white
TEXT_DARK = colors.HexColor("#1A2533")
ACCENT    = colors.HexColor("#2ECC71")

# ── Custom Flowables ──────────────────────────────────────────────────────────

class ColorBand(Flowable):
    """Full-width horizontal colour band."""
    def __init__(self, height, color):
        super().__init__()
        self.band_h = height
        self.color  = color

    def wrap(self, avail_w, avail_h):
        self.avail_w = avail_w
        return avail_w, self.band_h

    def draw(self):
        self.canv.setFillColor(self.color)
        self.canv.rect(0, 0, self.avail_w, self.band_h, stroke=0, fill=1)


class HeroBlock(Flowable):
    """Large hero banner at the top of the document."""
    def __init__(self, width, height):
        super().__init__()
        self.w = width
        self.h = height

    def wrap(self, avail_w, avail_h):
        return self.w, self.h

    def draw(self):
        c = self.canv
        # Background gradient-like effect via two rectangles
        c.setFillColor(NAVY)
        c.rect(0, 0, self.w, self.h, stroke=0, fill=1)

        # Decorative teal strip on left
        c.setFillColor(TEAL)
        c.rect(0, 0, 8, self.h, stroke=0, fill=1)

        # Gold accent bar at bottom
        c.setFillColor(GOLD)
        c.rect(8, 0, self.w - 8, 6, stroke=0, fill=1)

        # Decorative circle top-right
        c.setFillColor(colors.HexColor("#1B7F8E"))
        c.setFillAlpha(0.25)
        c.circle(self.w - 40, self.h - 40, 80, stroke=0, fill=1)
        c.setFillAlpha(1)

        # Tagline small
        c.setFillColor(GOLD)
        c.setFont("Helvetica-Bold", 11)
        c.drawString(24, self.h - 38, "FOREK INSTITUTE  |  POWERED BY")

        # Main title
        c.setFillColor(WHITE)
        c.setFont("Helvetica-Bold", 36)
        c.drawString(24, self.h - 78, "Forek Online")

        # Subtitle
        c.setFillColor(colors.HexColor("#BDD9E8"))
        c.setFont("Helvetica", 15)
        c.drawString(24, self.h - 102, "The Complete Occupational Training Management Platform")

        # Divider
        c.setStrokeColor(GOLD)
        c.setLineWidth(1.5)
        c.line(24, self.h - 116, self.w - 24, self.h - 116)

        # Three pillars
        pillars = [
            ("Enroll", "Manage every applicant from first click to enrolled student."),
            ("Train",  "Deliver lessons, assessments and workplace experience."),
            ("Report", "Track compliance, generate reports and drive outcomes."),
        ]
        col_w = (self.w - 48) / 3
        for i, (title, desc) in enumerate(pillars):
            x = 24 + i * col_w
            c.setFillColor(GOLD)
            c.setFont("Helvetica-Bold", 13)
            c.drawString(x + 4, self.h - 148, title)
            c.setFillColor(colors.HexColor("#BDD9E8"))
            c.setFont("Helvetica", 9)
            # Simple word wrap — split on spaces
            words = desc.split()
            line_buf, lines = [], []
            for w in words:
                test = " ".join(line_buf + [w])
                if c.stringWidth(test, "Helvetica", 9) < col_w - 12:
                    line_buf.append(w)
                else:
                    lines.append(" ".join(line_buf))
                    line_buf = [w]
            if line_buf:
                lines.append(" ".join(line_buf))
            for j, ln in enumerate(lines):
                c.drawString(x + 4, self.h - 163 - j * 13, ln)


class FeatureCard(Flowable):
    """Coloured card with an icon letter, title and bullets."""
    def __init__(self, icon, title, bullets, color=TEAL, width=None):
        super().__init__()
        self.icon    = icon
        self.title   = title
        self.bullets = bullets
        self.color   = color
        self._width  = width or 170 * mm

    def wrap(self, avail_w, avail_h):
        self.avail_w = self._width
        # Estimate height
        self.card_h  = 20 + 14 + len(self.bullets) * 14 + 16
        return self.avail_w, self.card_h

    def draw(self):
        c    = self.canv
        w, h = self.avail_w, self.card_h

        # Card background
        c.setFillColor(LIGHT_BG)
        c.roundRect(0, 0, w, h, 6, stroke=0, fill=1)

        # Left color accent
        c.setFillColor(self.color)
        c.roundRect(0, 0, 6, h, 3, stroke=0, fill=1)

        # Icon circle
        c.setFillColor(self.color)
        c.circle(24, h - 20, 12, stroke=0, fill=1)
        c.setFillColor(WHITE)
        c.setFont("Helvetica-Bold", 11)
        c.drawCentredString(24, h - 24, self.icon)

        # Title
        c.setFillColor(TEXT_DARK)
        c.setFont("Helvetica-Bold", 12)
        c.drawString(42, h - 22, self.title)

        # Divider
        c.setStrokeColor(MID_GREY)
        c.setLineWidth(0.4)
        c.line(42, h - 30, w - 8, h - 30)

        # Bullets
        c.setFont("Helvetica", 9.5)
        for i, b in enumerate(self.bullets):
            y = h - 44 - i * 14
            c.setFillColor(self.color)
            c.circle(48, y + 3, 2.5, stroke=0, fill=1)
            c.setFillColor(TEXT_DARK)
            c.drawString(55, y, b)


class StatBox(Flowable):
    """A single KPI stat box."""
    def __init__(self, number, label, color=TEAL):
        super().__init__()
        self.number = number
        self.label  = label
        self.color  = color

    def wrap(self, avail_w, avail_h):
        self.bw = avail_w
        return avail_w, 52

    def draw(self):
        c = self.canv
        c.setFillColor(self.color)
        c.roundRect(0, 0, self.bw, 52, 6, stroke=0, fill=1)
        c.setFillColor(WHITE)
        c.setFont("Helvetica-Bold", 22)
        c.drawCentredString(self.bw / 2, 28, self.number)
        c.setFont("Helvetica", 9)
        c.drawCentredString(self.bw / 2, 12, self.label)


# ── Styles ────────────────────────────────────────────────────────────────────

def make_styles():
    base = getSampleStyleSheet()

    def s(name, **kw):
        return ParagraphStyle(name, **kw)

    return {
        "section_heading": s("SH",
            fontName="Helvetica-Bold", fontSize=20,
            textColor=NAVY, spaceAfter=4),
        "section_sub": s("SS",
            fontName="Helvetica", fontSize=12,
            textColor=MID_GREY, spaceAfter=14),
        "body": s("BD",
            fontName="Helvetica", fontSize=10.5,
            textColor=TEXT_DARK, leading=16, spaceAfter=8,
            alignment=TA_JUSTIFY),
        "caption": s("CAP",
            fontName="Helvetica-Oblique", fontSize=9,
            textColor=MID_GREY, spaceAfter=6),
        "footer": s("FT",
            fontName="Helvetica", fontSize=8,
            textColor=MID_GREY, alignment=TA_CENTER),
        "quote": s("QT",
            fontName="Helvetica-Oblique", fontSize=13,
            textColor=TEAL, leading=18, spaceAfter=10,
            leftIndent=16, rightIndent=16),
        "bullet_title": s("BT",
            fontName="Helvetica-Bold", fontSize=11,
            textColor=NAVY, spaceAfter=2),
        "small": s("SM",
            fontName="Helvetica", fontSize=9,
            textColor=TEXT_DARK, leading=13),
        "toc_item": s("TOC",
            fontName="Helvetica", fontSize=11,
            textColor=NAVY, leftIndent=12, spaceAfter=6),
        "page_title": s("PT",
            fontName="Helvetica-Bold", fontSize=16,
            textColor=WHITE, alignment=TA_CENTER),
        "cover_tag": s("CT",
            fontName="Helvetica-Bold", fontSize=10,
            textColor=GOLD, spaceAfter=4),
    }


# ── Page template callbacks ───────────────────────────────────────────────────

def add_page_decorations(canvas, doc):
    """Footer on every page except page 1 (cover)."""
    if doc.page == 1:
        return
    canvas.saveState()
    w, h = A4
    canvas.setFillColor(NAVY)
    canvas.rect(0, 0, w, 22, stroke=0, fill=1)
    canvas.setFillColor(WHITE)
    canvas.setFont("Helvetica", 8)
    canvas.drawString(20, 7, "Forek Online  |  Occupational Training Management Platform")
    canvas.drawRightString(w - 20, 7, f"Page {doc.page}")
    # top accent line
    canvas.setFillColor(TEAL)
    canvas.rect(0, h - 4, w, 4, stroke=0, fill=1)
    canvas.restoreState()


# ── Build document ────────────────────────────────────────────────────────────

def build_pdf(output_path):
    doc = SimpleDocTemplate(
        output_path,
        pagesize=A4,
        leftMargin=18 * mm,
        rightMargin=18 * mm,
        topMargin=16 * mm,
        bottomMargin=24 * mm,
        title="Forek Online — Product Brochure",
        author="Forek Institute",
    )

    W = A4[0] - 36 * mm   # usable width
    styles = make_styles()
    story  = []

    # ── PAGE 1: COVER ─────────────────────────────────────────────────────────
    story.append(HeroBlock(W, 210))
    story.append(Spacer(1, 18))

    intro = (
        "Forek Online is a powerful, all-in-one digital platform built exclusively "
        "for occupational training providers. Whether you run a small skills academy "
        "or a multi-campus institute, Forek Online gives you everything you need to "
        "manage students, deliver training, track workplace learning and stay "
        "fully compliant — all from a single, easy-to-use system."
    )
    story.append(Paragraph(intro, styles["body"]))
    story.append(Spacer(1, 10))

    # Quick-stat row
    stat_data = [
        [StatBox("82+", "Data Modules"),
         StatBox("29",  "System Functions",  color=GOLD),
         StatBox("40+", "Report Types",      color=NAVY),
         StatBox("100%","Cloud-Ready",        color=ACCENT)],
    ]
    stat_tbl = Table(stat_data, colWidths=[W / 4] * 4, rowHeights=[60])
    stat_tbl.setStyle(TableStyle([
        ("LEFTPADDING",  (0,0), (-1,-1), 4),
        ("RIGHTPADDING", (0,0), (-1,-1), 4),
        ("TOPPADDING",   (0,0), (-1,-1), 0),
        ("BOTTOMPADDING",(0,0), (-1,-1), 0),
    ]))
    story.append(stat_tbl)
    story.append(Spacer(1, 16))

    story.append(HRFlowable(width=W, thickness=1, color=TEAL))
    story.append(Spacer(1, 8))
    story.append(Paragraph(
        "<i>Designed for training providers. Built for results. Trusted by Forek.</i>",
        styles["quote"]
    ))
    story.append(PageBreak())

    # ── PAGE 2: WHAT IS FOREK ONLINE? ────────────────────────────────────────
    story.append(ColorBand(6, TEAL))
    story.append(Spacer(1, 14))
    story.append(Paragraph("What Is Forek Online?", styles["section_heading"]))
    story.append(Paragraph(
        "A single platform that runs your entire training operation.", styles["section_sub"]
    ))

    story.append(Paragraph(
        "Running a training institute means juggling applications, enrolments, lesson "
        "plans, workplace visits, assessments, reports and compliance deadlines — often "
        "across multiple courses, campuses and staff members. Forek Online replaces "
        "spreadsheets, paper registers and disconnected systems with one secure, "
        "cloud-connected platform that your whole team can use from anywhere.",
        styles["body"]
    ))

    story.append(Spacer(1, 10))

    who_data = [
        [
            Paragraph("<b>Who it serves</b>", styles["bullet_title"]),
            Paragraph("<b>What they get</b>", styles["bullet_title"]),
        ],
        [
            Paragraph("Training Administrators", styles["small"]),
            Paragraph("Full control over applications, enrolments, courses and staff.", styles["small"]),
        ],
        [
            Paragraph("Lecturers &amp; Assessors", styles["small"]),
            Paragraph("Digital lesson plans, online assessments and instant results.", styles["small"]),
        ],
        [
            Paragraph("Workplace Mentors", styles["small"]),
            Paragraph("Timesheet reviews, visit logs and learner progress at a glance.", styles["small"]),
        ],
        [
            Paragraph("Students / Learners", styles["small"]),
            Paragraph("Online applications, access to learning materials and results.", styles["small"]),
        ],
        [
            Paragraph("Finance &amp; Compliance Officers", styles["small"]),
            Paragraph("Financial clearance, report compliance and automated reminders.", styles["small"]),
        ],
    ]

    who_tbl = Table(who_data, colWidths=[W * 0.35, W * 0.65])
    who_tbl.setStyle(TableStyle([
        ("BACKGROUND",   (0,0), (-1,0),  NAVY),
        ("TEXTCOLOR",    (0,0), (-1,0),  WHITE),
        ("FONTNAME",     (0,0), (-1,0),  "Helvetica-Bold"),
        ("FONTSIZE",     (0,0), (-1,0),  10),
        ("ROWBACKGROUNDS",(0,1),(-1,-1), [LIGHT_BG, WHITE]),
        ("GRID",         (0,0), (-1,-1), 0.4, MID_GREY),
        ("LEFTPADDING",  (0,0), (-1,-1), 8),
        ("RIGHTPADDING", (0,0), (-1,-1), 8),
        ("TOPPADDING",   (0,0), (-1,-1), 7),
        ("BOTTOMPADDING",(0,0), (-1,-1), 7),
        ("VALIGN",       (0,0), (-1,-1), "MIDDLE"),
    ]))
    story.append(who_tbl)
    story.append(Spacer(1, 14))

    story.append(Paragraph(
        "No technical background required. If you can send an email, you can use Forek Online.",
        styles["quote"]
    ))
    story.append(PageBreak())

    # ── PAGE 3: CORE FEATURES ─────────────────────────────────────────────────
    story.append(ColorBand(6, GOLD))
    story.append(Spacer(1, 14))
    story.append(Paragraph("Core Features", styles["section_heading"]))
    story.append(Paragraph(
        "Everything your institution needs, built in from day one.", styles["section_sub"]
    ))

    half = (W - 10) / 2

    features = [
        ("A", "Online Applications & Enrolment",
         ["Custom application cycles", "Online application portal for learners",
          "Automated status updates", "Document upload & verification",
          "Bulk enrolment management"],
         TEAL),
        ("B", "Student Lifecycle Management",
         ["Complete student profiles & history", "ID, address & contact details",
          "Document storage in the cloud", "Deregistration & re-enrolment",
          "Guardian / next-of-kin records"],
         NAVY),
        ("C", "Courses & Lesson Planning",
         ["17+ occupational qualifications supported", "Digital lesson plans per module",
          "Attendance tracking per lesson", "Learning resources & materials library",
          "Role-based content access"],
         TEAL),
        ("D", "Assessments & Results",
         ["Build question banks online", "Learner self-assessment portal",
          "Automatic marking & scoring", "Attempt history & analytics",
          "Assessor review workflow"],
         GOLD),
        ("E", "Workplace-Based Learning (WBL)",
         ["Placement & venue management", "Mentor-signed weekly timesheets",
          "Workplace visit logging", "WBL module tracking per learner",
          "Mentor review queue"],
         NAVY),
        ("F", "Compliance & Reporting",
         ["Automated report compliance tracking", "Grace-period & extension management",
          "Progress, submission & finance reports", "PDF export & download",
          "Compliance deadline reminders"],
         TEAL),
    ]

    for i in range(0, len(features), 2):
        left_args  = features[i]
        right_args = features[i+1] if i+1 < len(features) else None

        left_card  = FeatureCard(left_args[0], left_args[1], left_args[2],
                                 left_args[3], width=half)
        if right_args:
            right_card = FeatureCard(right_args[0], right_args[1], right_args[2],
                                     right_args[3], width=half)
            row = Table([[left_card, right_card]], colWidths=[half, half])
        else:
            row = Table([[left_card, ""]], colWidths=[half, half])

        row.setStyle(TableStyle([
            ("LEFTPADDING",  (0,0),(-1,-1), 0),
            ("RIGHTPADDING", (0,0),(-1,-1), 0),
            ("TOPPADDING",   (0,0),(-1,-1), 0),
            ("BOTTOMPADDING",(0,0),(-1,-1), 8),
            ("ALIGN",        (0,0),(-1,-1), "LEFT"),
            ("VALIGN",       (0,0),(-1,-1), "TOP"),
            ("INNERGRID",    (0,0),(-1,-1), 0, colors.white),
            ("BOX",          (0,0),(-1,-1), 0, colors.white),
            ("COLUMNPADDING",(0,0),(-1,-1), 5),
        ]))
        story.append(row)

    story.append(PageBreak())

    # ── PAGE 4: FINANCE, COMMS & ADMIN ───────────────────────────────────────
    story.append(ColorBand(6, NAVY))
    story.append(Spacer(1, 14))
    story.append(Paragraph("Finance, Communications & Administration", styles["section_heading"]))
    story.append(Paragraph("The back-office tools that keep everything running.", styles["section_sub"]))

    fin_body = (
        "Forek Online goes beyond academics. It integrates the administrative and "
        "financial workflows that training providers deal with every day."
    )
    story.append(Paragraph(fin_body, styles["body"]))
    story.append(Spacer(1, 8))

    admin_rows = [
        ["Finance Management",
         "Track learner finance records, issue financial clearance, manage payslip "
         "requests and maintain full audit trails for every financial interaction."],
        ["Notifications & Alerts",
         "Send in-app notifications, emails and SMS messages to learners, staff "
         "and mentors — automatically triggered by system events or manually by admin."],
        ["Document Management",
         "Store, retrieve and share all documents securely in the cloud (Azure Blob "
         "Storage). Supports learner documents, evidence, contracts and more."],
        ["User & Role Management",
         "Granular role-based access control ensures that every user — student, "
         "mentor, assessor or admin — sees only what they need to see."],
        ["Background Jobs & Automation",
         "Automated tasks run silently in the background: syncing student data from "
         "external systems, sending scheduled reminders and processing bulk imports."],
        ["Audit Trail & Security",
         "Every action is logged. Full audit history ensures accountability and "
         "supports SETA compliance requirements."],
    ]

    tbl_data = [[Paragraph(f"<b>{r[0]}</b>", styles["bullet_title"]),
                 Paragraph(r[1], styles["small"])] for r in admin_rows]

    admin_tbl = Table(tbl_data, colWidths=[W * 0.30, W * 0.70])
    admin_tbl.setStyle(TableStyle([
        ("ROWBACKGROUNDS", (0,0),(-1,-1), [LIGHT_BG, WHITE]),
        ("GRID",           (0,0),(-1,-1), 0.4, MID_GREY),
        ("LEFTPADDING",    (0,0),(-1,-1), 8),
        ("RIGHTPADDING",   (0,0),(-1,-1), 8),
        ("TOPPADDING",     (0,0),(-1,-1), 8),
        ("BOTTOMPADDING",  (0,0),(-1,-1), 8),
        ("VALIGN",         (0,0),(-1,-1), "TOP"),
    ]))
    story.append(admin_tbl)
    story.append(Spacer(1, 16))

    story.append(Paragraph(
        "One login. Every tool your team needs.",
        styles["quote"]
    ))
    story.append(PageBreak())

    # ── PAGE 5: HOW IT WORKS (WORKFLOW) ──────────────────────────────────────
    story.append(ColorBand(6, TEAL))
    story.append(Spacer(1, 14))
    story.append(Paragraph("How It Works", styles["section_heading"]))
    story.append(Paragraph("From first enquiry to qualified learner — in one system.", styles["section_sub"]))

    steps = [
        ("1", TEAL,  "Learner Applies Online",
         "Applicants complete their application through the online portal. "
         "They upload required documents and receive automated status updates throughout."),
        ("2", GOLD,  "Admin Reviews & Enrols",
         "Administrators review applications, verify documents and confirm enrolment "
         "with a few clicks. Learner profiles are created automatically."),
        ("3", NAVY,  "Training Begins",
         "Lecturers deliver lessons tracked on the platform. Learners access materials, "
         "complete assessments online and view their results instantly."),
        ("4", TEAL,  "Workplace Learning",
         "Learners are placed with employers. Mentors log weekly timesheets and visits "
         "through the platform. Everything is signed off digitally."),
        ("5", GOLD,  "Reports & Compliance",
         "The system tracks compliance deadlines automatically. Progress reports are "
         "generated with a single click and submitted digitally."),
        ("6", NAVY,  "Outcomes & Certification",
         "Finance clearance is confirmed, records are finalised and the learner's "
         "journey is archived for SETA and audit purposes."),
    ]

    for num, col, title, desc in steps:
        row_content = [
            [_step_box(num, col, self_width=38),
             [Paragraph(f"<b>{title}</b>", styles["bullet_title"]),
              Paragraph(desc, styles["small"])]],
        ]
        tbl = Table([[_step_box(num, col), Table([[Paragraph(f"<b>{title}</b>", styles["bullet_title"])],[Paragraph(desc, styles["small"])]])]], colWidths=[46, W - 46])
        tbl.setStyle(TableStyle([
            ("VALIGN",       (0,0),(-1,-1), "TOP"),
            ("LEFTPADDING",  (0,0),(-1,-1), 0),
            ("RIGHTPADDING", (0,0),(-1,-1), 0),
            ("TOPPADDING",   (0,0),(-1,-1), 4),
            ("BOTTOMPADDING",(0,0),(-1,-1), 4),
            ("INNERGRID",    (0,0),(-1,-1), 0, WHITE),
            ("BOX",          (0,0),(-1,-1), 0, WHITE),
        ]))
        story.append(tbl)
        story.append(Spacer(1, 4))

    story.append(PageBreak())

    # ── PAGE 6: TECHNOLOGY & SECURITY ────────────────────────────────────────
    story.append(ColorBand(6, GOLD))
    story.append(Spacer(1, 14))
    story.append(Paragraph("Technology & Security", styles["section_heading"]))
    story.append(Paragraph("Enterprise-grade infrastructure. Simple for users.", styles["section_sub"]))

    story.append(Paragraph(
        "Forek Online is built on Microsoft's .NET 8 platform — the same technology "
        "trusted by banks, hospitals and governments worldwide. Your data is stored "
        "securely on Microsoft Azure cloud infrastructure with regular backups, "
        "role-based access controls and full audit logging.",
        styles["body"]
    ))
    story.append(Spacer(1, 8))

    tech_items = [
        ("Cloud Storage",         "All files and documents stored on Microsoft Azure — "
                                  "accessible from anywhere, backed up automatically."),
        ("Secure Login",          "Cookie-based authentication with role-based access. "
                                  "Every user sees only what they are permitted to see."),
        ("Data Integrity",        "Every change to the system is logged with a timestamp "
                                  "and the user who made it — full audit trail built in."),
        ("Background Processing", "Automated jobs run 24/7: import student data, send "
                                  "reminders and process bulk operations without interruption."),
        ("Reporting Engine",      "Powerful report generation powered by Stimulsoft — "
                                  "export to PDF, Excel or view on screen."),
        ("SMS & Email Alerts",    "Integrated with WinSMS and SMTP for real-time "
                                  "communication with learners, mentors and staff."),
    ]

    tech_rows = [[Paragraph(f"<b>{t[0]}</b>", styles["bullet_title"]),
                  Paragraph(t[1], styles["small"])] for t in tech_items]

    tech_tbl = Table(tech_rows, colWidths=[W * 0.28, W * 0.72])
    tech_tbl.setStyle(TableStyle([
        ("ROWBACKGROUNDS", (0,0),(-1,-1), [LIGHT_BG, WHITE]),
        ("GRID",           (0,0),(-1,-1), 0.4, MID_GREY),
        ("LEFTPADDING",    (0,0),(-1,-1), 8),
        ("RIGHTPADDING",   (0,0),(-1,-1), 8),
        ("TOPPADDING",     (0,0),(-1,-1), 8),
        ("BOTTOMPADDING",  (0,0),(-1,-1), 8),
        ("VALIGN",         (0,0),(-1,-1), "TOP"),
    ]))
    story.append(tech_tbl)
    story.append(Spacer(1, 14))

    # Trust badges as a simple coloured row
    badges = ["Microsoft Azure", ".NET 8 Platform", "SQL Server", "SETA-Aligned", "POPIA Compliant"]
    badge_cells = [[Paragraph(f"<b>{b}</b>", ParagraphStyle("badge",
        fontName="Helvetica-Bold", fontSize=9,
        textColor=WHITE, alignment=TA_CENTER))] for b in badges]

    badge_tbl = Table([badge_cells[0:]], colWidths=[W / len(badges)] * len(badges))
    badge_tbl = Table([badges], colWidths=[W / len(badges)] * len(badges))
    badge_tbl.setStyle(TableStyle([
        ("BACKGROUND",   (0,0),(-1,-1), NAVY),
        ("TEXTCOLOR",    (0,0),(-1,-1), WHITE),
        ("FONTNAME",     (0,0),(-1,-1), "Helvetica-Bold"),
        ("FONTSIZE",     (0,0),(-1,-1), 9),
        ("ALIGN",        (0,0),(-1,-1), "CENTER"),
        ("VALIGN",       (0,0),(-1,-1), "MIDDLE"),
        ("TOPPADDING",   (0,0),(-1,-1), 10),
        ("BOTTOMPADDING",(0,0),(-1,-1), 10),
        ("LINEAFTER",    (0,0),(-2,-1), 0.5, TEAL),
    ]))
    story.append(badge_tbl)
    story.append(PageBreak())

    # ── PAGE 7: WHY CHOOSE FOREK ONLINE ──────────────────────────────────────
    story.append(ColorBand(6, NAVY))
    story.append(Spacer(1, 14))
    story.append(Paragraph("Why Choose Forek Online?", styles["section_heading"]))
    story.append(Paragraph("The smarter way to run a training provider.", styles["section_sub"]))

    reasons = [
        ("Save Time",
         "Automate repetitive tasks — applications, reminders, imports and reporting — "
         "so your team can focus on what matters most: the learners."),
        ("Reduce Errors",
         "Replace paper registers, spreadsheets and email chains with a single source "
         "of truth. No more lost documents or missed deadlines."),
        ("Stay Compliant",
         "Built with SETA compliance in mind. Track report deadlines, manage grace "
         "periods and generate submission-ready documentation at any time."),
        ("Scale With Ease",
         "Whether you have 50 learners or 5 000, Forek Online scales with your "
         "institution without requiring additional IT infrastructure."),
        ("Work From Anywhere",
         "Fully web-based and cloud-hosted. Your staff, mentors and learners can "
         "access the system from any device, anywhere in the world."),
        ("All-in-One Value",
         "No need to pay for separate systems for HR, finance, LMS and reporting. "
         "Forek Online covers every function in a single subscription."),
    ]

    reason_cells = []
    for idx in range(0, len(reasons), 2):
        left  = reasons[idx]
        right = reasons[idx+1] if idx+1 < len(reasons) else ("", "")
        reason_cells.append([
            _reason_cell(left[0], left[1]),
            _reason_cell(right[0], right[1]) if right[0] else "",
        ])

    reason_tbl = Table(reason_cells, colWidths=[W/2, W/2])
    reason_tbl.setStyle(TableStyle([
        ("LEFTPADDING",  (0,0),(-1,-1), 4),
        ("RIGHTPADDING", (0,0),(-1,-1), 4),
        ("TOPPADDING",   (0,0),(-1,-1), 0),
        ("BOTTOMPADDING",(0,0),(-1,-1), 8),
        ("VALIGN",       (0,0),(-1,-1), "TOP"),
    ]))
    story.append(reason_tbl)
    story.append(Spacer(1, 14))

    story.append(Paragraph(
        "\"Forek Online puts the whole training operation in one place — "
        "from the first application to the final report.\"",
        styles["quote"]
    ))
    story.append(PageBreak())

    # ── PAGE 8: CALL TO ACTION / BACK COVER ──────────────────────────────────
    story.append(ColorBand(6, TEAL))
    story.append(Spacer(1, 24))

    story.append(Paragraph("Ready to Transform Your Training Institute?", styles["section_heading"]))
    story.append(Spacer(1, 6))
    story.append(Paragraph(
        "Join the institutions already using Forek Online to deliver better outcomes "
        "for their learners, reduce administrative burden and meet every compliance "
        "deadline with confidence.",
        styles["body"]
    ))
    story.append(Spacer(1, 20))

    cta_items = [
        ("Request a Demo",   "See Forek Online live — we'll walk you through the full system."),
        ("Get a Quote",      "Pricing tailored to your institution size and requirements."),
        ("Talk to Our Team", "Have questions? Our team is ready to help."),
    ]

    cta_rows = [[
        Table([
            [Paragraph(f"<b>{c[0]}</b>",
                ParagraphStyle("cta_t", fontName="Helvetica-Bold", fontSize=12,
                               textColor=WHITE))],
            [Paragraph(c[1],
                ParagraphStyle("cta_d", fontName="Helvetica", fontSize=9,
                               textColor=colors.HexColor("#BDD9E8")))],
        ], colWidths=[(W/3) - 12])
    ] for c in cta_items]

    cta_data = [[
        Table(cta_rows[0], colWidths=[(W/3)-12]),
        Table(cta_rows[1], colWidths=[(W/3)-12]),
        Table(cta_rows[2], colWidths=[(W/3)-12]),
    ]]

    # Simpler flat CTA table
    flat_cta = []
    for title, desc in cta_items:
        flat_cta.append([
            Paragraph(f"<b>{title}</b>",
                ParagraphStyle("ctt", fontName="Helvetica-Bold", fontSize=13, textColor=GOLD)),
            Paragraph(desc,
                ParagraphStyle("ctd", fontName="Helvetica", fontSize=10, textColor=TEXT_DARK)),
        ])

    cta_tbl = Table(flat_cta, colWidths=[W * 0.30, W * 0.70])
    cta_tbl.setStyle(TableStyle([
        ("ROWBACKGROUNDS", (0,0),(-1,-1), [LIGHT_BG, WHITE]),
        ("GRID",           (0,0),(-1,-1), 0.4, MID_GREY),
        ("LEFTPADDING",    (0,0),(-1,-1), 12),
        ("RIGHTPADDING",   (0,0),(-1,-1), 12),
        ("TOPPADDING",     (0,0),(-1,-1), 12),
        ("BOTTOMPADDING",  (0,0),(-1,-1), 12),
        ("VALIGN",         (0,0),(-1,-1), "MIDDLE"),
    ]))
    story.append(cta_tbl)
    story.append(Spacer(1, 30))

    # Contact block
    contact_data = [[
        Paragraph("<b>Forek Institute</b>", ParagraphStyle("cb", fontName="Helvetica-Bold", fontSize=11, textColor=NAVY)),
        Paragraph("www.forek.co.za", ParagraphStyle("cv", fontName="Helvetica", fontSize=10, textColor=TEAL)),
        Paragraph("info@forek.co.za", ParagraphStyle("ce", fontName="Helvetica", fontSize=10, textColor=TEAL)),
    ]]
    contact_tbl = Table(contact_data, colWidths=[W/3, W/3, W/3])
    contact_tbl.setStyle(TableStyle([
        ("BACKGROUND",   (0,0),(-1,-1), LIGHT_BG),
        ("ALIGN",        (0,0),(-1,-1), "CENTER"),
        ("VALIGN",       (0,0),(-1,-1), "MIDDLE"),
        ("TOPPADDING",   (0,0),(-1,-1), 14),
        ("BOTTOMPADDING",(0,0),(-1,-1), 14),
        ("BOX",          (0,0),(-1,-1), 0.5, MID_GREY),
    ]))
    story.append(contact_tbl)
    story.append(Spacer(1, 16))

    story.append(Paragraph(
        "Forek Online — Empowering Training Providers to Deliver Excellence.",
        ParagraphStyle("tagline", fontName="Helvetica-Bold", fontSize=13,
                       textColor=NAVY, alignment=TA_CENTER)
    ))

    # Build
    doc.build(story, onFirstPage=add_page_decorations, onLaterPages=add_page_decorations)
    print(f"PDF created: {output_path}")


# ── Helper builders ───────────────────────────────────────────────────────────

class _StepBox(Flowable):
    def __init__(self, number, color):
        super().__init__()
        self.number = number
        self.color  = color

    def wrap(self, aw, ah):
        return 38, 38

    def draw(self):
        c = self.canv
        c.setFillColor(self.color)
        c.circle(19, 19, 17, stroke=0, fill=1)
        c.setFillColor(WHITE)
        c.setFont("Helvetica-Bold", 16)
        c.drawCentredString(19, 13, self.number)


def _step_box(number, color, self_width=38):
    return _StepBox(number, color)


def _reason_cell(title, desc):
    return Table([
        [Paragraph(f"<b>{title}</b>",
            ParagraphStyle("rt", fontName="Helvetica-Bold", fontSize=11, textColor=TEAL))],
        [Paragraph(desc,
            ParagraphStyle("rd", fontName="Helvetica", fontSize=9.5,
                           textColor=TEXT_DARK, leading=14))],
    ], colWidths=[(174*mm)/2 - 8])


# ── Entry point ───────────────────────────────────────────────────────────────
if __name__ == "__main__":
    out = os.path.join(
        r"C:\Users\Itumelengo\OneDrive - FOSCHINI RETAIL GROUP PTY LTD\Desktop\FO16",
        "Forek_Online_Brochure.pdf"
    )
    build_pdf(out)
