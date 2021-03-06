﻿// CoreUtil
// 
// Copyright (C) 2012-2014 Daiyuu Nobori. All Rights Reserved.
// Copyright (C) 2012-2014 SoftEther VPN Project at University of Tsukuba. All Rights Reserved.
// Comments: Tetsuo Sugiyama, Ph.D.
// 
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// version 2 as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License version 2
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// THE LICENSE AGREEMENT IS ATTACHED ON THE SOURCE-CODE PACKAGE
// AS "LICENSE.TXT" FILE. READ THE TEXT FILE IN ADVANCE TO USE THE SOFTWARE.
// 
// 
// THIS SOFTWARE IS DEVELOPED IN JAPAN, AND DISTRIBUTED FROM JAPAN,
// UNDER JAPANESE LAWS. YOU MUST AGREE IN ADVANCE TO USE, COPY, MODIFY,
// MERGE, PUBLISH, DISTRIBUTE, SUBLICENSE, AND/OR SELL COPIES OF THIS
// SOFTWARE, THAT ANY JURIDICAL DISPUTES WHICH ARE CONCERNED TO THIS
// SOFTWARE OR ITS CONTENTS, AGAINST US (SOFTETHER PROJECT, SOFTETHER
// CORPORATION, DAIYUU NOBORI OR OTHER SUPPLIERS), OR ANY JURIDICAL
// DISPUTES AGAINST US WHICH ARE CAUSED BY ANY KIND OF USING, COPYING,
// MODIFYING, MERGING, PUBLISHING, DISTRIBUTING, SUBLICENSING, AND/OR
// SELLING COPIES OF THIS SOFTWARE SHALL BE REGARDED AS BE CONSTRUED AND
// CONTROLLED BY JAPANESE LAWS, AND YOU MUST FURTHER CONSENT TO
// EXCLUSIVE JURISDICTION AND VENUE IN THE COURTS SITTING IN TOKYO,
// JAPAN. YOU MUST WAIVE ALL DEFENSES OF LACK OF PERSONAL JURISDICTION
// AND FORUM NON CONVENIENS. PROCESS MAY BE SERVED ON EITHER PARTY IN
// THE MANNER AUTHORIZED BY APPLICABLE LAW OR COURT RULE.
// 
// USE ONLY IN JAPAN. DO NOT USE IT IN OTHER COUNTRIES. IMPORTING THIS
// SOFTWARE INTO OTHER COUNTRIES IS AT YOUR OWN RISK. SOME COUNTRIES
// PROHIBIT ENCRYPTED COMMUNICATIONS. USING THIS SOFTWARE IN OTHER
// COUNTRIES MIGHT BE RESTRICTED.
// 
// 
// DEAR SECURITY EXPERTS
// ---------------------
// 
// If you find a bug or a security vulnerability please kindly inform us
// about the problem immediately so that we can fix the security problem
// to protect a lot of users around the world as soon as possible.
// 
// Our e-mail address for security reports is:
// softether-vpn-security [at] softether.org
// 
// Please note that the above e-mail address is not a technical support
// inquiry address. If you need technical assistance, please visit
// http://www.softether.org/ and ask your question on the users forum.
// 
// Thank you for your cooperation.


using System;
using System.Threading;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.Mail;
using CoreUtil.Internal;

namespace CoreUtil
{
	public static class ZLib
	{
		public static byte[] Compress(byte[] src)
		{
			return Compress(src, zlibConst.Z_DEFAULT_COMPRESSION);
		}
		public static byte[] Compress(byte[] src, int level)
		{
			return Compress(src, level, false);
		}
		public static byte[] Compress(byte[] src, int level, bool noHeader)
		{
			int dstSize = src.Length * 2 + 100;
			byte[] dst = new byte[dstSize];

			compress2(ref dst, src, level, noHeader);

			return dst;
		}

		public static byte[] Uncompress(byte[] src, int originalSize)
		{
			byte[] dst = new byte[originalSize];

			uncompress(ref dst, src);

			return dst;
		}

		static void compress2(ref byte[] dest, byte[] src, int level, bool noHeader)
		{
			ZStream stream = new ZStream();

			stream.next_in = src;
			stream.avail_in = src.Length;

			stream.next_out = dest;
			stream.avail_out = dest.Length;

			if (noHeader == false)
			{
				stream.deflateInit(level);
			}
			else
			{
				stream.deflateInit(level, -15);
			}

			stream.deflate(zlibConst.Z_FINISH);

			Array.Resize<byte>(ref dest, (int)stream.total_out);
		}

		static void uncompress(ref byte[] dest, byte[] src)
		{
			ZStream stream = new ZStream();

			stream.next_in = src;
			stream.avail_in = src.Length;

			stream.next_out = dest;
			stream.avail_out = dest.Length;

			stream.inflateInit();

			int err = stream.inflate(zlibConst.Z_FINISH);
			if (err != zlibConst.Z_STREAM_END)
			{
				stream.inflateEnd();
				throw new ApplicationException();
			}

			Array.Resize<byte>(ref dest, (int)stream.total_out);

			err = stream.inflateEnd();
			if (err != zlibConst.Z_OK)
			{
				throw new ApplicationException();
			}
		}
	}
}

// Developed by SoftEther VPN Project at University of Tsukuba in Japan.
// Department of Computer Science has dozens of overly-enthusiastic geeks.
// Join us: http://www.tsukuba.ac.jp/english/admission/
