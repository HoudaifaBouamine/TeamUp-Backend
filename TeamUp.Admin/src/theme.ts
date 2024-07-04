
import type { CustomThemeConfig } from '@skeletonlabs/tw-plugin';

export const myCustomTheme: CustomThemeConfig = {
    name: 'my-custom-theme',
    properties: {
		// =~= Theme Properties =~=
		"--theme-font-family-base": `system-ui`,
		"--theme-font-family-heading": `system-ui`,
		"--theme-font-color-base": "0 0 0",
		"--theme-font-color-dark": "255 255 255",
		"--theme-rounded-base": "9999px",
		"--theme-rounded-container": "8px",
		"--theme-border-base": "1px",
		// =~= Theme On-X Colors =~=
		"--on-primary": "255 255 255",
		"--on-secondary": "255 255 255",
		"--on-tertiary": "0 0 0",
		"--on-success": "0 0 0",
		"--on-warning": "0 0 0",
		"--on-error": "255 255 255",
		"--on-surface": "255 255 255",
		// =~= Theme Colors  =~=
		// primary | #682DFE 
		"--color-primary-50": "232 224 255", // #e8e0ff
		"--color-primary-100": "225 213 255", // #e1d5ff
		"--color-primary-200": "217 203 255", // #d9cbff
		"--color-primary-300": "195 171 255", // #c3abff
		"--color-primary-400": "149 108 254", // #956cfe
		"--color-primary-500": "104 45 254", // #682DFE
		"--color-primary-600": "94 41 229", // #5e29e5
		"--color-primary-700": "78 34 191", // #4e22bf
		"--color-primary-800": "62 27 152", // #3e1b98
		"--color-primary-900": "51 22 124", // #33167c
		// secondary | #070707 
		"--color-secondary-50": "218 218 218", // #dadada
		"--color-secondary-100": "205 205 205", // #cdcdcd
		"--color-secondary-200": "193 193 193", // #c1c1c1
		"--color-secondary-300": "156 156 156", // #9c9c9c
		"--color-secondary-400": "81 81 81", // #515151
		"--color-secondary-500": "7 7 7", // #070707
		"--color-secondary-600": "6 6 6", // #060606
		"--color-secondary-700": "5 5 5", // #050505
		"--color-secondary-800": "4 4 4", // #040404
		"--color-secondary-900": "3 3 3", // #030303
		// tertiary | #EFF2FB 
		"--color-tertiary-50": "253 253 254", // #fdfdfe
		"--color-tertiary-100": "252 252 254", // #fcfcfe
		"--color-tertiary-200": "251 252 254", // #fbfcfe
		"--color-tertiary-300": "249 250 253", // #f9fafd
		"--color-tertiary-400": "244 246 252", // #f4f6fc
		"--color-tertiary-500": "239 242 251", // #EFF2FB
		"--color-tertiary-600": "215 218 226", // #d7dae2
		"--color-tertiary-700": "179 182 188", // #b3b6bc
		"--color-tertiary-800": "143 145 151", // #8f9197
		"--color-tertiary-900": "117 119 123", // #75777b
		// success | #84cc16 
		"--color-success-50": "237 247 220", // #edf7dc
		"--color-success-100": "230 245 208", // #e6f5d0
		"--color-success-200": "224 242 197", // #e0f2c5
		"--color-success-300": "206 235 162", // #ceeba2
		"--color-success-400": "169 219 92", // #a9db5c
		"--color-success-500": "132 204 22", // #84cc16
		"--color-success-600": "119 184 20", // #77b814
		"--color-success-700": "99 153 17", // #639911
		"--color-success-800": "79 122 13", // #4f7a0d
		"--color-success-900": "65 100 11", // #41640b
		// warning | #EAB308 
		"--color-warning-50": "252 244 218", // #fcf4da
		"--color-warning-100": "251 240 206", // #fbf0ce
		"--color-warning-200": "250 236 193", // #faecc1
		"--color-warning-300": "247 225 156", // #f7e19c
		"--color-warning-400": "240 202 82", // #f0ca52
		"--color-warning-500": "234 179 8", // #EAB308
		"--color-warning-600": "211 161 7", // #d3a107
		"--color-warning-700": "176 134 6", // #b08606
		"--color-warning-800": "140 107 5", // #8c6b05
		"--color-warning-900": "115 88 4", // #735804
		// error | #D41976 
		"--color-error-50": "249 221 234", // #f9ddea
		"--color-error-100": "246 209 228", // #f6d1e4
		"--color-error-200": "244 198 221", // #f4c6dd
		"--color-error-300": "238 163 200", // #eea3c8
		"--color-error-400": "225 94 159", // #e15e9f
		"--color-error-500": "212 25 118", // #D41976
		"--color-error-600": "191 23 106", // #bf176a
		"--color-error-700": "159 19 89", // #9f1359
		"--color-error-800": "127 15 71", // #7f0f47
		"--color-error-900": "104 12 58", // #680c3a
		// surface | #070707 
		"--color-surface-50": "218 218 218", // #dadada
		"--color-surface-100": "205 205 205", // #cdcdcd
		"--color-surface-200": "193 193 193", // #c1c1c1
		"--color-surface-300": "156 156 156", // #9c9c9c
		"--color-surface-400": "81 81 81", // #515151
		"--color-surface-500": "7 7 7", // #070707
		"--color-surface-600": "6 6 6", // #060606
		"--color-surface-700": "5 5 5", // #050505
		"--color-surface-800": "4 4 4", // #040404
		"--color-surface-900": "3 3 3", // #030303
		
	}
}