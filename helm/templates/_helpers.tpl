{{- define "people-web-app.name" -}}
{{- printf "%s" .Chart.Name | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "people-web-app.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "people-web-app.labels" -}}
helm.sh/chart: {{ include "people-web-app.chart" . }}
{{ include "people-web-app.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "people-web-app.selectorLabels" -}}
app.kubernetes.io/name: {{ include "people-web-app.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}
