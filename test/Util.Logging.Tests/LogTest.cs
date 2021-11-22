using Moq;

namespace Util.Logging.Tests {
    /// <summary>
    /// ��־��������
    /// </summary>
    public partial class LogTest {
        /// <summary>
        /// ģ����־
        /// </summary>
        private readonly Mock<ILoggerWrapper<LogTest>> _mockLogger;
        /// <summary>
        /// ��־����
        /// </summary>
        private readonly ILog<LogTest> _log;

        /// <summary>
        /// ���Գ�ʼ��
        /// </summary>
        public LogTest() {
            _mockLogger = new Mock<ILoggerWrapper<LogTest>>();
            _log = new Log<LogTest>( _mockLogger.Object );
        }
    }
}
