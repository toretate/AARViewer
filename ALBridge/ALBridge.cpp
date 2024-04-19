// これは メイン DLL ファイルです。

#include "stdafx.h"

#include "ALBridge.h"

const READ_DIR = "データファイルの場所を絶対パスで"
const AAR_FILE = "ダンプ元のAARファイル名(READ_DIR内のファイル)"

namespace bridge {
	public ref class ALBridge {
	public:
		ALBridge() {
			const char *path = READ_DIR;
			ALStream::SetBasePath(path);	// ALライブラリにベースパス設定を行う
		}

		void dump()
		{
			ALArchive *alar = ALArchive::Create(AAR_FILE);	// AARファイルの読み込み
			const Uint32 count = alar->GetCount();
			// count取って何したかったんだろうか…
		}

	};
}
