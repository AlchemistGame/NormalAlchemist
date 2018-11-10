## 工作流

远程仓库有 master 和 develop 两条分支.

平时所有的开发内容都是合到 develop 分支.

只有在发布版本时, 才从 develop 分支合并到 master 分支,
并打一个 tag, 以 `v1.0` 的形式 ( 版本号 ) 进行命名.

如果发布版本出了问题, 新建一条 `bugfix/YOUR_BUG_NAME` 分支, 直接合到 master 分支 ( 当然, 这条分支的内容同样需要合到 develop )

## 工具

推荐采用 [TortoiseGit](https://tortoisegit.org/) ( 别名"小乌龟" ) 或 [Sourcetree](https://www.sourcetreeapp.com/) 对 Git 进行管理


## 操作习惯

#### 合并时推荐使用 rebase ( 当然 merge 也是可以接受的 )

如我们在本地 develop 分支开发了新东西, 即创建了一个 commit,

先 fetch 获取远程仓库最新内容 ( 这将自动合到 origin/develop 分支 ),

然后从本地 develop 分支 rebase 上游分支 origin/develop,

解决冲突后, push 到远程仓库.

#### 如果要回退远程仓库, 用 revert 而不是 reset

reset 会删减 commit 记录 ( 提交记录会消失在 log 中 ), 如果对上游分支做了 reset 操作, 下游分支的提交记录会乱掉.

revert 则是创建一条新的 commit 记录, 抵消掉之前的提交记录, 即 log 中的提交记录还是在往前走, 下游分支更新时就不会出错.
